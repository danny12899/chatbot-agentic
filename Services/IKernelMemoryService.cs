using Microsoft.KernelMemory;

namespace chatbot_agentic.Services
{
    public interface IKernelMemoryService
    {
        IKernelMemory GetKernelMemory();
    }
}
