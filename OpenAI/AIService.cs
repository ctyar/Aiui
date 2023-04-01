using OpenAI.GPT3;
using OpenAI.GPT3.Managers;
using OpenAI.GPT3.ObjectModels.RequestModels;

namespace OpenAI;

public class AIService
{
    private readonly OpenAIService _openAIService;

    public AIService(IConfiguration configuration)
    {
        _openAIService = new OpenAIService(new OpenAiOptions()
        {
            ApiKey = configuration.GetValue<string>("OpenApiKey") ?? throw new Exception("OpenApiKey missing")
        });
    }

    public async Task<string> GetAsync(string prompt)
    {
        var completionResult = await _openAIService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
        {
            Messages = new List<ChatMessage>
            {
                ChatMessage.FromSystem("You are a helpful assistant."),
                //ChatMessage.FromUser("Who won the world series in 2020?"),
                //ChatMessage.FromAssistant("The Los Angeles Dodgers won the World Series in 2020."),
                ChatMessage.FromUser(prompt)
            },
            Model = GPT3.ObjectModels.Models.ChatGpt3_5Turbo,
        });

        if (!completionResult.Successful)
        {
            throw new Exception(completionResult.Error?.Message);
        }

        return completionResult.Choices.First().Message.Content;
    }
}
