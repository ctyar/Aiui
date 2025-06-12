using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Logging;

namespace Aiui;

internal sealed class AzureOpenAIService : IOpenAIService
{
    private readonly OpenAIClient _openAIClient;
    private readonly Collection<IPlugin> _plugins;
    private readonly ILogger _logger;

    public AzureOpenAIService(OpenAIClient openAIClient, Collection<IPlugin> plugins, ILogger logger)
    {
        _openAIClient = openAIClient;
        _plugins = plugins;
        _logger = logger;
    }

    public async Task<Result?> GetAsync(string prompt, List<Message> chatHistory, object? context)
    {
        var chatCompletionsOptions = new ChatCompletionsOptions()
        {
            DeploymentName = "gpt-4.1",
            Temperature = 0f,
        };

        chatCompletionsOptions.Messages.Add(new ChatRequestSystemMessage("You are a software developer"));
        chatCompletionsOptions.Messages.Add(new ChatRequestSystemMessage("Do not give comment or explanation"));

        foreach (var plugin in _plugins)
        {
            chatCompletionsOptions.Functions.Add(plugin.GetFunctionDefinition());

            var pluginPrompts = await plugin.BuildPromptAsync(context, _logger);
            if (pluginPrompts is null)
            {
                continue;
            }
            foreach (var pluginPrompt in pluginPrompts)
            {
                chatCompletionsOptions.Messages.Add(GetChatMessage(pluginPrompt));
            }
        }

        foreach (var chat in chatHistory.Where(item => item.Type == MessageType.User || item.Type == MessageType.AI))
        {
            chatCompletionsOptions.Messages.Add(GetChatMessage(chat));
        }

        chatCompletionsOptions.Messages.Add(new ChatRequestUserMessage(prompt));

        var responseChatCompletions = await _openAIClient.GetChatCompletionsAsync(chatCompletionsOptions);

        var chatChoice = responseChatCompletions.Value.Choices[0];

        if (chatChoice.FinishReason == CompletionsFinishReason.FunctionCall)
        {
            // TODO: move this to each plugin
            chatCompletionsOptions.Messages.Add(
                new ChatRequestAssistantMessage("SQL query successfully executed. The result is in memory now.")
                {
                    Name = chatChoice.Message.FunctionCall.Name
                });
            var responseChatCompletions2 = await _openAIClient.GetChatCompletionsAsync(chatCompletionsOptions);

            return new Result(ResultType.PluginExecution, chatChoice.Message.FunctionCall.Name, chatChoice.Message.FunctionCall.Arguments);
        }

        return new Result(ResultType.Message, chatChoice.Message.Content);
    }

    private static ChatRequestMessage GetChatMessage(Message message)
    {
        if (message.Type == MessageType.User)
        {
            return new ChatRequestUserMessage(message.Content);
        }
        else if (message.Type == MessageType.AI)
        {
            return new ChatRequestSystemMessage(message.Content);
        }

        throw new NotSupportedException();
    }
}

internal sealed class Result
{
    public ResultType Type { get; }
    public string Value { get; }
    public string? PluginArguments { get; }

    public Result(ResultType type, string value, string? pluginArguments = null)
    {
        Type = type;
        Value = value;
        PluginArguments = pluginArguments;
    }
}

internal enum ResultType
{
    PluginExecution,
    Message
}
