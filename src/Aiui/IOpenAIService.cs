using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aiui;

internal interface IOpenAIService
{
    Task<Result?> GetAsync(string prompt, List<Message> chatHistory, object? context);
}
