using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel;
using chatbot_agentic.Util;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel.Agents;
using Markdig;

namespace chatbot_agentic.Services
{
    public class AzureService : IAzureService
    {
        private readonly AppSettings _appSettings;
        private readonly IKernelService _kernelService;
        private readonly IKernelMemoryService _kernelMemoryService;

        public AzureService(IOptions<AppSettings> appSettings,
            IKernelService kernelService,
            IKernelMemoryService kernelMemoryService)
        {
            _appSettings = appSettings.Value;
            _kernelService = kernelService;
            _kernelMemoryService = kernelMemoryService;
        }

        public async Task<string> AskQuestion(string prompt, string documentId)
        {
            var kernel = _kernelService.GetKernel();

            var agent = new ChatCompletionAgent
            {
                Name = "ChatAgent",
                Instructions = "Answer questions",
                Kernel = kernel
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
