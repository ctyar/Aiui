using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.AI;

namespace Aiui;

public sealed class ChartJsPlugin : IPlugin
{
    public Task<ChatMessage?> GetRootPromptAsync()
    {
        return Task.FromResult<ChatMessage?>(null);
    }

    public Task<ChatMessage?> GetContextPromptAsync(object? context)
    {
        if (context is null)
        {
            return Task.FromResult<ChatMessage?>(null);
        }

        if (context is not List<dynamic> rows)
        {
            return Task.FromResult<ChatMessage?>(null);
        }

        var row = rows.First() as IDictionary<string, object>;

        var columns = string.Join(",", row?.Keys ?? Array.Empty<string>());

        var textContent = new TextContent($"We have a JavaScript variable named 'context' which is an array of objects with these properties: {columns}. " +
            "This variable `context` holds the data for the ChartJS.");

        return Task.FromResult<ChatMessage?>(new ChatMessage
        {
            Role = ChatRole.System,
            Contents = [textContent]
        });
    }

    public Task<object?> ExecuteAsync(string aiResponse)
    {
        if (aiResponse is null)
        {
            return Task.FromResult<object?>(null);
        }

        return Task.FromResult<object?>(aiResponse);
    }
}
