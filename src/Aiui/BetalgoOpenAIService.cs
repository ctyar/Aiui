namespace Aiui;

/*internal sealed class BetalgoOpenAIService : IOpenAIService
{
    private readonly string _apiKey;

    public BetalgoOpenAIService(string apiKey)
    {
        _apiKey = apiKey;
    }

    public async Task<string?> GetAsync(List<Message> pluginPrompts, List<Message> chatHistory)
    {
        var openAiService = new OpenAIService(new OpenAiOptions()
        {
            ApiKey = _apiKey
        });

        var chatCompletionCreateRequest = new ChatCompletionCreateRequest
        {
            Model = Models.Gpt_3_5_Turbo,
            Temperature = 0f
        };

        chatCompletionCreateRequest.Messages.Add(ChatMessage.FromSystem("You are a software developer"));

        chatCompletionCreateRequest.Messages.Add(ChatMessage.FromSystem("Do not give comment or explanation"));

        foreach (var prompt in pluginPrompts)
        {
            chatCompletionCreateRequest.Messages.Add(GetChatMessage(prompt));
        }

        foreach (var chat in chatHistory.Where(item => item.Type == MessageType.User || item.Type == MessageType.System))
        {
            chatCompletionCreateRequest.Messages.Add(GetChatMessage(chat));
        }

        // TODO: Implement
        *//*chatCompletionCreateRequest.Functions.Add(new FunctionDefinition
        {
            Description
        })*//*

        var completionResult = await openAiService.ChatCompletion.CreateCompletion(chatCompletionCreateRequest);

        return completionResult.Choices[0].Message.Content;
    }

    private static ChatMessage GetChatMessage(Message message)
    {
        return message.Type switch
        {
            MessageType.User => ChatMessage.FromUser(message.Content),
            MessageType.System => ChatMessage.FromSystem(message.Content),
        };
    }
}*/
