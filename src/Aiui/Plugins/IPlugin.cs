using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Logging;

namespace Aiui;

public interface IPlugin
{
    FunctionDefinition GetFunctionDefinition();

    Task<List<Message>?> BuildPromptAsync(object? context, ILogger logger);

    Task<object?> GetResultAsync(string aiResponse, ILogger logger);
}
