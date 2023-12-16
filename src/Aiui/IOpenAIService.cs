using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aiui;

internal interface IOpenAIService
{
    Task<Result?> GetAsync(IEnumerable<IPlugin> plugins, string prompt, List<Message> chatHistory, object? context);
}
