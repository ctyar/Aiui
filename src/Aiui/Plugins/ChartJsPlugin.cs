using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Logging;

namespace Aiui;

public sealed class ChartJsPlugin : IPlugin
{
    public FunctionDefinition GetFunctionDefinition()
    {
        return new FunctionDefinition
        {
            Name = nameof(ChartJsPlugin),
            Description = "A function that receives the JavaScript code needed for Chart.js as string and draws the chart for the data in memory",
            Parameters = BinaryData.FromObjectAsJson(new
            {
                type = "object",
                properties = new Dictionary<string, object>()
                {
                    {
                        nameof(Arguments.ChartJsCode), new
                        {
                            type = "string",
                            description = "The JavaScript code needed for Chart.js as string to draw a chart in an HTML canvas element " +
                                " with the id 'myChart'"
                        }
                    }
                },
                required = new[] { nameof(Arguments.ChartJsCode) }
            })
        };
    }

    public Task<List<Message>?> BuildPromptAsync(object? context, ILogger logger)
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
                    Type = MessageType.AI,
                    Content = $"We have a JavaScript variable named 'context' which is an array of objects with these properties: {columns}. This variable `context` holds the data for the ChartJS."
                });
            }
        }

        return Task.FromResult(result)!;
    }

    public Task<object?> GetResultAsync(string aiResponse, ILogger logger)
    {
        if (aiResponse is null)
        {
            return Task.FromResult((object?)null);
        }

        return Task.FromResult((object?)aiResponse);
    }

    private class Arguments
    {
        public string? ChartJsCode { get; set; }
    }
}
