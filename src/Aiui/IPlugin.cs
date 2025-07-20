using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aiui;

public interface IPlugin
{
    Task<List<Message>> GetRootPromptAsync();

    Task<List<Message>> GetContextPromptAsync(object? context);

    Task<object?> ExecuteAsync(string aiResponse);
}
