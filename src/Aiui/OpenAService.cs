using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.AI.OpenAI;

namespace Aiui;

internal sealed class OpenAIService
{
    private readonly OpenAIClient _openAIClient;

    public OpenAIService(OpenAIClient openAIClient)
    {
        _openAIClient = openAIClient;
    }

    public async Task<string?> GetAsync(List<ChatMessage> pluginPrompts, List<Message> chatHistory)
    {
        var chatCompletionsOptions = new ChatCompletionsOptions()
        {
            Temperature = 0f
        };

        chatCompletionsOptions.Messages.Add(new ChatMessage(ChatRole.System, "You are a software developer"));

        chatCompletionsOptions.Messages.Add(new ChatMessage(ChatRole.System, "Do not give comment or explanation"));

        foreach (var prompt in pluginPrompts)
        {
            chatCompletionsOptions.Messages.Add(prompt);
        }

        foreach (var chat in chatHistory.Where(item => item.Type == MessageType.User || item.Type == MessageType.System))
        {
            var role = chat.Type switch
            {
                MessageType.User => ChatRole.User,
                MessageType.System => ChatRole.System,
            };
            chatCompletionsOptions.Messages.Add(new ChatMessage(role, chat.Content));
        }

        var responseChatCompletions = await _openAIClient.GetChatCompletionsAsync("gpt-3.5-turbo", chatCompletionsOptions);

        return responseChatCompletions.Value.Choices[0].Message.Content;
    }
}
