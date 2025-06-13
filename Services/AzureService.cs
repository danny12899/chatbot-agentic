using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel;
using chatbot_agentic.Util;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel.Agents;
using Markdig;
using Azure;
using Azure.Identity;
using Azure.AI.Projects;
using Azure.AI.Agents.Persistent;
using System.Text;
using chatbot_agentic.Agents;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace chatbot_agentic.Services
{
    public class AzureService : IAzureService
    {
        private readonly AppSettings _appSettings;
        private readonly IFunctionInvocationFilter _functionInvocationFilter;
        private readonly IKernelService _kernelService;
        private readonly IKernelMemoryService _kernelMemoryService;

        public AzureService(IOptions<AppSettings> appSettings,
            IFunctionInvocationFilter functionInvocationFilter,
            IKernelService kernelService,
            IKernelMemoryService kernelMemoryService)
        {
            _appSettings = appSettings.Value;
            _functionInvocationFilter = functionInvocationFilter;
            _kernelService = kernelService;
            _kernelMemoryService = kernelMemoryService;
        }

        public async Task<string> AskCodeInterpreter(string prompt)
        {
            var cred = new DefaultAzureCredential();
            var endpoint = new Uri(_appSettings.AzureAgentProjectUrl);
            
            AIProjectClient projectClient = new AIProjectClient(endpoint, new DefaultAzureCredential());
            
            PersistentAgentsClient agentsClient = projectClient.GetPersistentAgentsClient();

            PersistentAgent persistentAgent = agentsClient.Administration.CreateAgent(
                    model: _appSettings.ChatCompletionModelId,
                    name: "Code Interpreter Agent",
                    instructions: "Write and run Python code to answer the question. In your answer include the Python code you wrote.",
                    tools: [new CodeInterpreterToolDefinition()]
                );
            
            PersistentAgentThread thread = agentsClient.Threads.CreateThread();

            var agentResponse = agentsClient.Messages.CreateMessage(thread.Id, MessageRole.User, prompt);
            
            ThreadRun run = agentsClient.Runs.CreateRun(thread.Id, persistentAgent.Id);

            do
            {
                await Task.Delay(TimeSpan.FromMilliseconds(500));
                run = agentsClient.Runs.GetRun(thread.Id, run.Id);
            }
            while (run.Status == RunStatus.Queued
                    || run.Status == RunStatus.InProgress);

            Pageable<PersistentThreadMessage> messages = agentsClient.Messages.GetMessages(thread.Id, order: ListSortOrder.Ascending);

            var sb = new StringBuilder();
            foreach (PersistentThreadMessage message in messages)
            {
                if (message.Role == MessageRole.Agent)
                    foreach (MessageContent contentItem in message.ContentItems)
                    {
                        if (contentItem is MessageTextContent textItem)
                        {
                            sb.Append(textItem.Text);
                        }
                    }
            }

            agentsClient.Threads.DeleteThread(threadId: thread.Id);
            agentsClient.Administration.DeleteAgent(agentId: persistentAgent.Id);

            return Markdown.ToHtml(sb.ToString());
        }

        public async Task<string> AskQuestion(string prompt, string documentId)
        {
            var kernel = _kernelService.GetKernel();
            kernel.FunctionInvocationFilters.Add(new FunctionInvocationFilter());

            var functionFromPrompt = kernel.CreateFunctionFromPrompt("When asked what time or date is it reply with 'I do not have that information'");

            kernel.ImportPluginFromFunctions("my_plugin", [functionFromPrompt]);


            var agent = new ChatCompletionAgent
            {
                Name = "ChatAgent",
                Instructions = "Answer questions",
                Kernel = kernel,
                Arguments = new KernelArguments(
                new OpenAIPromptExecutionSettings()
                {
                    FunctionChoiceBehavior = FunctionChoiceBehavior.Required()
                })
            };

            var response = string.Empty;
            await foreach (var agentResponse in agent.InvokeAsync(new ChatMessageContent(AuthorRole.User, prompt)))
            {
                response += agentResponse.Message;
            }

            return Markdown.ToHtml(response);
        }

        public async Task<string> ImportDocumentAsync(Stream stream, string fileName)
        {
            var km = _kernelMemoryService.GetKernelMemory();
            return await km.ImportDocumentAsync(stream, fileName);
        }
    }
}
