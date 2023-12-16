using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Logging;

namespace Aiui;

public sealed class BotService
{
    private readonly ILogger<BotService> _logger;

    public BotService(ILogger<BotService> logger)
    {
        _logger = logger;
    }

    public Task<ExecutionResult> ExecutePromptAsync(IEnumerable<IPlugin> plugins, OpenAIClient openAIClient, string prompt, List<Message> chatHistory,
        object? context)
    {
        ArgumentNullException.ThrowIfNull(openAIClient);

        var azureOpenAIService = new AzureOpenAIService(openAIClient, _logger);

        return ExecutePromptAsync(plugins, azureOpenAIService, prompt, chatHistory, context);
    }

    private async Task<ExecutionResult> ExecutePromptAsync(IEnumerable<IPlugin> plugins, IOpenAIService openAIService, string prompt,
        List<Message> chatHistory, object? context)
    {
        ArgumentNullException.ThrowIfNull(plugins);
        ArgumentNullException.ThrowIfNull(prompt);
        ArgumentNullException.ThrowIfNull(chatHistory);

        var newHistory = GetNewHistory(prompt, chatHistory);

        var response = await openAIService.GetAsync(plugins, prompt, chatHistory, context);

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

        var plugin = plugins.FirstOrDefault(p => p.GetFunctionDefinition().Name == response.Value);

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
