using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Aiui;

public sealed class ChartJsPlugin : IPlugin
{
    public Task<List<Message>?> BuildPromptAsync(string prompt, object? context, ILogger logger)
    {
        var result = new List<Message>();

        if (context != null)
        {
            if (context is List<dynamic> rows)
            {
                var row = rows.First() as IDictionary<string, object>;

                var columns = string.Join(",", row?.Keys ?? Array.Empty<string>());
                result.Add(new Message
                {
                    Type = MessageType.System,
                    Content = $"Imagine we have an object named data which is an array of objects with these properties: {columns}"
                });
            }
        }

        result.Add(new Message
        {
            Type = MessageType.System,
            Content = "We also have a HTML canvas with the id 'myChart'"
        });

        result.Add(new Message
        {
            Type = MessageType.System,
            Content = "When instructed to draw a chart, generate the JavaScript needed for Chart.js using the above information"
        });

        result.Add(new Message
        {
            Type = MessageType.System,
            Content = "When creating the JavaScript code you must be brief and no explanation just write the JavaScript code itself and nothing else, this is very important"
        });

        result.Add(new Message
        {
            Type = MessageType.System,
            Content = $"{prompt}, no explanation"
        });

        return Task.FromResult(result)!;
    }

    public Task<object?> GetResultAsync(string aiResponse, ILogger logger)
    {
        var jsCode = Clean(aiResponse);

        return Task.FromResult((object?)jsCode);
    }

    private static string Clean(string aiResponse)
    {
        // Remove ``` anywhere in the query
        aiResponse = aiResponse.Replace("```js", "");
        aiResponse = aiResponse.Replace("```javascript", "");
        aiResponse = aiResponse.Replace("```", "");

        return aiResponse;
    }
}
