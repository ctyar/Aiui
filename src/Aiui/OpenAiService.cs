using System.Collections.Generic;
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

    public async Task<string?> GetAsync(List<string> schema, string prompt, List<string> chatHistory)
    {
        var chatCompletionsOptions = new ChatCompletionsOptions();

        foreach (var item in schema)
        {
            chatCompletionsOptions.Messages.Add(new ChatMessage(ChatRole.System, item));
        }

        chatCompletionsOptions.Messages.Add(new ChatMessage(ChatRole.System,
            "When instructed to list or show or create a report create a SQL query with the above knowledge instead"));

        chatCompletionsOptions.Messages.Add(new ChatMessage(ChatRole.System,
            "When creating a SQL query you must be brief and no explanation just write the SQL query itself and nothing else, this is very important"));

        foreach (var chat in chatHistory)
        {
            chatCompletionsOptions.Messages.Add(new ChatMessage(ChatRole.User, chat));
        }
        chatCompletionsOptions.Messages.Add(new ChatMessage(ChatRole.User, prompt));

        var responseChatCompletions = await _openAIClient.GetChatCompletionsAsync("gpt-3.5-turbo", chatCompletionsOptions);

        return responseChatCompletions.Value.Choices[0].Message.Content;
    }
}
