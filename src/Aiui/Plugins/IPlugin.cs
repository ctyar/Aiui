using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Aiui;

public interface IPlugin
{
    Task<List<Message>?> BuildPromptAsync(string prompt, object? context, ILogger logger);

    Task<object?> GetResultAsync(string aiResponse, ILogger logger);
}
