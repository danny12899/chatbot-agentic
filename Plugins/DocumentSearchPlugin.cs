using System.ComponentModel;
using Microsoft.KernelMemory;
using Microsoft.SemanticKernel;

namespace chatbot_agentic.Plugins
{
    public class DocumentSearchPlugin
    {
        private readonly string _documentId;
        private KernelFunction _fn;
        private readonly IKernelMemory _km;

        public DocumentSearchPlugin(IKernelMemory km, string documentId)
        {
            _documentId = documentId;
            _km = km;
        }

        [KernelFunction("Ask"), Description("Use memory to answer question")]
        public async Task<string> AskAsync(
            [Description("Question to answer")]
            string input
            )
        {
            var filters = new List<MemoryFilter>();
            filters.Add(MemoryFilters.ByDocument(_documentId));

            var response = await _km.AskAsync(question: input, filters: filters);
            return response.Result;
        }
    }
}
