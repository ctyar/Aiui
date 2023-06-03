using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Logging;

namespace Aiui;

public interface IPlugin
{
    Task<List<ChatMessage>?> BuildPromptAsync(string prompt, object? context, ILogger logger);

    Task<object?> GetResultAsync(string aiResponse, ILogger logger);
}
