using chatbot_agentic.Util;
using Microsoft.Extensions.Options;
using Microsoft.KernelMemory;

namespace chatbot_agentic.Services
{
    public class KernelMemoryService : IKernelMemoryService
    {
        private readonly AppSettings _appSettings;

        public KernelMemoryService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public IKernelMemory GetKernelMemory()
        {
            return new KernelMemoryBuilder()
                .WithAzureOpenAITextGeneration(new AzureOpenAIConfig
                {
                    APIKey = _appSettings.ApiKey,
                    Endpoint = _appSettings.Endpoint,
                    Deployment = _appSettings.ChatCompletionModelId,
                    APIType = AzureOpenAIConfig.APITypes.ChatCompletion,
                    Auth = AzureOpenAIConfig.AuthTypes.APIKey
                })
                .WithAzureOpenAITextEmbeddingGeneration(new AzureOpenAIConfig
                {
                    APIKey = _appSettings.ApiKey,
                    Endpoint = _appSettings.Endpoint,
                    Deployment = _appSettings.EmbeddingModelId,
                    APIType = AzureOpenAIConfig.APITypes.EmbeddingGeneration,
                    Auth = AzureOpenAIConfig.AuthTypes.APIKey
                })
                .WithSqlServerMemoryDb(_appSettings.ConnectionString)
                .Build(new KernelMemoryBuilderBuildOptions { AllowMixingVolatileAndPersistentData = true });
        }

        public async Task<string> ImportDocumentAsync(Stream stream, string fileName)
        {
            var kernel = GetKernelMemory();
            return await kernel.ImportDocumentAsync(stream, fileName);
        }
    }
}
