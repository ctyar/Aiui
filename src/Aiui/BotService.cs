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

    public async Task<ExecutionResult> ExecutePromptAsync(IPlugin plugin, OpenAIClient openAIClient, string prompt, List<Message> chatHistory, object? context)
    {
        ArgumentNullException.ThrowIfNull(plugin);
        ArgumentNullException.ThrowIfNull(openAIClient);
        ArgumentNullException.ThrowIfNull(prompt);
        ArgumentNullException.ThrowIfNull(chatHistory);

        var newHistory = GetNewHistory(prompt, chatHistory);

        var pluginPrompts = await plugin.BuildPromptAsync(prompt, context, _logger);

        if (pluginPrompts is null)
        {
            return new ExecutionResult(chatHistory);
        }

        var openAIService = new OpenAIService(openAIClient);
        var response = await openAIService.GetAsync(pluginPrompts, chatHistory);

        if (response is null)
        {
            return new ExecutionResult(newHistory);
        }

        var data = await plugin.GetResultAsync(response, _logger);

        if (data is null)
        {
            // Probably just a normal command response
            newHistory.Add(new Message
            {
                Type = MessageType.System,
                Content = response
            });

            return new ExecutionResult(newHistory, response);
        }
        else
        {
            newHistory.Add(new Message
            {
                Type = MessageType.Info,
                Content = "Done"
            });
        }

        return new ExecutionResult(newHistory, response, data);
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
