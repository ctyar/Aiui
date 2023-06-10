using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Logging;

namespace Aiui;

public sealed class ChartJsPlugin : IPlugin
{
    public Task<List<ChatMessage>?> BuildPromptAsync(string prompt, object? context, ILogger logger)
    {
        var result = new List<ChatMessage>();

        if (context != null)
        {
            if (context is List<dynamic> rows)
            {
                var row = rows.First() as IDictionary<string, object>;

                var columns = string.Join(",", row?.Keys ?? Array.Empty<string>());
                result.Add(new ChatMessage(ChatRole.System, $"Imagine we have an object named data which is an array of objects with these properties: {columns}"));
            }
        }

        result.Add(new ChatMessage(ChatRole.System,
            "When instructed to draw a chart, generate the JavaScript needed for Chart.js with the above knowledge instead"));

        result.Add(new ChatMessage(ChatRole.System,
            "When creating the JavaScript code you must be brief and no explanation just write the JavaScript code itself and nothing else, this is very important"));

        result.Add(new ChatMessage(ChatRole.User, $"{prompt}, no explanation"));

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
        aiResponse = aiResponse.Replace("```", "");

        return aiResponse;
    }
}
