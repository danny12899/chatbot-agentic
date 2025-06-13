using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace chatbot_agentic.Plugins
{
    public class DatePlugin
    {
        [KernelFunction("get_date")]
        [Description("Gets current date.")]
        public string GetCurrentDate()
        {
            return DateTime.Now.ToShortDateString();
        }
    }
}
