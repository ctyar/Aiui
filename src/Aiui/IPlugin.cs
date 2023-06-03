using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Logging;

namespace Aiui;

public interface IPlugin
{
    Task<List<ChatMessage>?> BuildPromptAsync(string prompt, ILogger logger);

    Task<List<dynamic>?> GetDataAsync(string aiResponse, ILogger logger);
}
