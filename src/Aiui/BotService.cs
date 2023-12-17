using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Aiui;

public sealed class BotService
{
    private readonly ILogger<BotService> _logger;
    private readonly AiuiOptions _options;

    public BotService(ILogger<BotService> logger, AiuiOptions options)
    {
        _logger = logger;
        _options = options;
    }

    public Task<ExecutionResult> ExecutePromptAsync(string prompt, List<Message> chatHistory, object? context)
    {
        ArgumentNullException.ThrowIfNull(prompt);
        ArgumentNullException.ThrowIfNull(chatHistory);

        var azureOpenAIService = new AzureOpenAIService(_options.Client, _options.Plugins, _logger);

        return ExecutePromptAsync(azureOpenAIService, prompt, chatHistory, context);
    }

    private async Task<ExecutionResult> ExecutePromptAsync(IOpenAIService openAIService, string prompt, List<Message> chatHistory,
        object? context)
    {
        var newHistory = GetNewHistory(prompt, chatHistory);

        var response = await openAIService.GetAsync(prompt, chatHistory, context);

        if (response is null)
        {
            return new ExecutionResult(newHistory);
        }

        if (response.Type == ResultType.Message)
        {
            newHistory.Add(new Message
            {
                Type = MessageType.AI,
                Content = response.Value
            });

            return new ExecutionResult(newHistory, response.Value);
        }

        var plugin = _options.Plugins.FirstOrDefault(p => p.GetFunctionDefinition().Name == response.Value);

        if (plugin is null)
        {
            return new ExecutionResult(newHistory, response.Value + response.PluginArguments);
        }

        var data = await plugin.GetResultAsync(response.PluginArguments!, _logger);

        if (data is null)
        {
            return new ExecutionResult(newHistory, response.Value + response.PluginArguments);
        }

        newHistory.Add(new Message
        {
            Type = MessageType.Aiui,
            Content = "Done"
        });

        return new ExecutionResult(newHistory, response.Value + response.PluginArguments, data);
    }

    private static List<Message> GetNewHistory(string prompt, List<Message> chatHistory)
    {
        var newChatHistory = chatHistory.ToList();

        newChatHistory.Add(new Message
        {
            Type = MessageType.User,
            Content = prompt
        });

        return newChatHistory;
    }
}
