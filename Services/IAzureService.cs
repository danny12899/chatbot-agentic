namespace chatbot_agentic.Services
{
    public interface IAzureService
    {
        Task<string> AskQuestion(string prompt, string documentId);
        Task<string> ImportDocumentAsync(Stream stream, string fileName);
    }
}
