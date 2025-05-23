using Microsoft.SemanticKernel;

namespace chatbot_agentic.Services
{
    public interface IKernelService
    {
        Kernel GetKernel();
    }
}
