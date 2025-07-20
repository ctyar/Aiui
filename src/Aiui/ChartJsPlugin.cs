using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiui;

public sealed class ChartJsPlugin : IPlugin
{
    public Task<List<Message>> GetRootPromptAsync()
    {
        return Task.FromResult<List<Message>>([]);
    }

    public Task<List<Message>> GetContextPromptAsync(object? context)
    {
        var result = new List<Message>();

        if (context is null)
        {
            return Task.FromResult(result);
        }

        if (context is List<dynamic> rows)
        {
            var row = rows.First() as IDictionary<string, object>;

            var columns = string.Join(",", row?.Keys ?? Array.Empty<string>());
            result.Add(new Message
            {
                Type = MessageType.AI,
                Content = $"We have a JavaScript variable named 'context' which is an array of objects with these properties: {columns}. " +
                    "This variable `context` holds the data for the ChartJS."
            });
        }

        return Task.FromResult(result)!;
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
