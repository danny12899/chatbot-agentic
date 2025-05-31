namespace chatbot_agentic.Util
{
    public class AppSettings
    {
        public string Endpoint { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string ChatCompletionModelId { get; set; } = string.Empty;
        public string EmbeddingModelId { get; set; } = string.Empty;
        public string ConnectionString { get; set; } = string.Empty;
        public string AzureAgentProjectUrl { get; set; } = string.Empty;
    }
}
