using chatbot_agentic.Util;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;

namespace chatbot_agentic.Services
{
    public class KernelService : IKernelService
    {
        private readonly AppSettings _appSettings;

        public KernelService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public Kernel GetKernel()
        {
            // Create a kernel with Azure OpenAI chat completion
            var builder = Kernel.CreateBuilder().AddAzureOpenAIChatCompletion(_appSettings.ChatCompletionModelId, _appSettings.Endpoint, _appSettings.ApiKey);

            // Add enterprise components
            builder.Services.AddLogging(services => services.AddConsole().SetMinimumLevel(LogLevel.Trace));

            // Build the kernel
            return builder.Build();
        }
    }
}
