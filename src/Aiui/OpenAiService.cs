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

    public async Task<string?> GetAsync(List<string> schema, string prompt, List<Message> chatHistory)
    {
        var chatCompletionsOptions = new ChatCompletionsOptions()
        {
            Temperature = 0f
        };

        chatCompletionsOptions.Messages.Add(new ChatMessage(ChatRole.System,
            "You are a software developer"));

        foreach (var tableSchema in schema)
        {
            chatCompletionsOptions.Messages.Add(new ChatMessage(ChatRole.System, tableSchema));
        }

        chatCompletionsOptions.Messages.Add(new ChatMessage(ChatRole.System,
            "When instructed to list or show or create a report create a SQL query with the above knowledge instead"));

        chatCompletionsOptions.Messages.Add(new ChatMessage(ChatRole.System,
            "When creating a SQL query you must be brief and no explanation just write the SQL query itself and nothing else, this is very important"));

        chatCompletionsOptions.Messages.Add(new ChatMessage(ChatRole.System,
            "Do not give comment or explanation"));

        foreach (var chat in chatHistory.Where(item => item.Type == MessageType.User || item.Type == MessageType.System))
        {
            var role = chat.Type switch
            {
                MessageType.User => ChatRole.User,
                MessageType.System => ChatRole.System,
            };
            chatCompletionsOptions.Messages.Add(new ChatMessage(role, chat.Content));
        }
        chatCompletionsOptions.Messages.Add(new ChatMessage(ChatRole.User, prompt));

        var responseChatCompletions = await _openAIClient.GetChatCompletionsAsync("gpt-3.5-turbo", chatCompletionsOptions);

        return responseChatCompletions.Value.Choices[0].Message.Content;
    }
}
