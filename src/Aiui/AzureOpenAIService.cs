using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.AI.OpenAI;

namespace Aiui;

internal sealed class AzureOpenAIService : IOpenAIService
{
    private readonly OpenAIClient _openAIClient;

    public AzureOpenAIService(OpenAIClient openAIClient)
    {
        _openAIClient = openAIClient;
    }

    public async Task<string?> GetAsync(List<Message> pluginPrompts, List<Message> chatHistory)
    {
        var chatCompletionsOptions = new ChatCompletionsOptions()
        {
            Temperature = 0f,
        };

        chatCompletionsOptions.Messages.Add(new ChatMessage(ChatRole.System, "You are a software developer"));

        chatCompletionsOptions.Messages.Add(new ChatMessage(ChatRole.System, "Do not give comment or explanation"));

        foreach (var prompt in pluginPrompts)
        {
            chatCompletionsOptions.Messages.Add(GetChatMessage(prompt));
        }

        foreach (var chat in chatHistory.Where(item => item.Type == MessageType.User || item.Type == MessageType.System))
        {
            chatCompletionsOptions.Messages.Add(GetChatMessage(chat));
        }

        var responseChatCompletions = await _openAIClient.GetChatCompletionsAsync("gpt-3.5-turbo", chatCompletionsOptions);

        return responseChatCompletions.Value.Choices[0].Message.Content;
    }

    private static ChatMessage GetChatMessage(Message message)
    {
        var role = message.Type switch
        {
            MessageType.User => ChatRole.User,
            MessageType.System => ChatRole.System,
        };

        return new ChatMessage(role, message.Content);
    }
}
