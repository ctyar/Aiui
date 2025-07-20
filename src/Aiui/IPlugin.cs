using System.Threading.Tasks;
using Microsoft.Extensions.AI;

namespace Aiui;

public interface IPlugin
{
    Task<ChatMessage?> GetRootPromptAsync();

    Task<ChatMessage?> GetContextPromptAsync(object? context);

    Task<object?> ExecuteAsync(string aiResponse);
}
