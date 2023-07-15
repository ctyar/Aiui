using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aiui;

internal interface IOpenAIService
{
    Task<string?> GetAsync(List<Message> pluginPrompts, List<Message> chatHistory);
}
