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

    public async Task<ExecutionResult> ExecutePromptAsync(IPlugin plugin, OpenAIClient openAIClient, string prompt, List<Message> chatHistory)
    {
        ArgumentNullException.ThrowIfNull(plugin);
        ArgumentNullException.ThrowIfNull(openAIClient);
        ArgumentNullException.ThrowIfNull(prompt);
        ArgumentNullException.ThrowIfNull(chatHistory);

        var newHistory = GetNewHistory(prompt, chatHistory);

        var pluginPrompts = await plugin.BuildPromptAsync(prompt, _logger);

        if (pluginPrompts is null)
        {
            return new ExecutionResult(chatHistory);
        }

        var openAiService = new OpenAIService(openAIClient);
        var aiResponse = await openAiService.GetAsync(pluginPrompts, chatHistory);

        if (aiResponse is null)
        {
            return new ExecutionResult(newHistory);
        }

        var data = await plugin.GetDataAsync(aiResponse, _logger);

        if (data is null)
        {
            // Probably just a normal command response
            newHistory.Add(new Message
            {
                Type = MessageType.System,
                Content = aiResponse
            });

            return new ExecutionResult(newHistory, aiResponse);
        }
        else
        {
            newHistory.Add(new Message
            {
                Type = MessageType.Info,
                Content = "Done"
            });
        }

        return new ExecutionResult(newHistory, aiResponse, data);
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
