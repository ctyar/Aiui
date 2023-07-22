using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Logging;

namespace Aiui;

internal sealed class AzureOpenAIService : IOpenAIService
{
    private readonly OpenAIClient _openAIClient;
    private readonly ILogger _logger;

    public AzureOpenAIService(OpenAIClient openAIClient, ILogger logger)
    {
        _openAIClient = openAIClient;
        _logger = logger;
    }

    public async Task<Result?> GetAsync(IPlugin plugin, string prompt, List<Message> chatHistory, object? context)
    {
        var chatCompletionsOptions = new ChatCompletionsOptions()
        {
            Temperature = 0f,
        };

        chatCompletionsOptions.Messages.Add(new ChatMessage(ChatRole.System, "You are a software developer"));

        chatCompletionsOptions.Messages.Add(new ChatMessage(ChatRole.System, "Do not give comment or explanation"));

        chatCompletionsOptions.Functions.Add(plugin.GetFunctionDefinition());

        var pluginPrompts = await plugin.BuildPromptAsync(context, _logger);

        if (pluginPrompts is null)
        {
            return null;
        }
        foreach (var pluginPrompt in pluginPrompts)
        {
            chatCompletionsOptions.Messages.Add(GetChatMessage(pluginPrompt));
        }

        foreach (var chat in chatHistory.Where(item => item.Type == MessageType.User || item.Type == MessageType.AI))
        {
            chatCompletionsOptions.Messages.Add(GetChatMessage(chat));
        }

        chatCompletionsOptions.Messages.Add(new ChatMessage(ChatRole.User, prompt));


        var responseChatCompletions = await _openAIClient.GetChatCompletionsAsync("gpt-3.5-turbo", chatCompletionsOptions);


        var chatChoice = responseChatCompletions.Value.Choices[0];

        if (chatChoice.FinishReason == CompletionsFinishReason.FunctionCall)
        {
            return new Result(ResultType.PluginExecution, chatChoice.Message.FunctionCall.Name, chatChoice.Message.FunctionCall.Arguments);
        }

        return new Result(ResultType.Message, chatChoice.Message.Content);
    }

    private static ChatMessage GetChatMessage(Message message)
    {
        var role = message.Type switch
        {
            MessageType.User => ChatRole.User,
            MessageType.AI => ChatRole.System,
        };

        return new ChatMessage(role, message.Content);
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
