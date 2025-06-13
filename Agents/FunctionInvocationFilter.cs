using System.Text.Json;
using Microsoft.SemanticKernel;

namespace chatbot_agentic.Agents
{
    public sealed class FunctionInvocationFilter : IFunctionInvocationFilter
    {
        public async Task OnFunctionInvocationAsync(FunctionInvocationContext context, Func<FunctionInvocationContext, Task> next)
        {
            try
            {
                await next(context);

                if (context.Result.ValueType == typeof(ChatMessageContent[]))
                {
                    Console.WriteLine($"{context.Function.Name} : {JsonSerializer.Serialize(context.Result.GetValue<ChatMessageContent[]>())}");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString() );
            }
        }
    }
}
