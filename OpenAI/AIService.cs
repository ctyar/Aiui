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
                ChatMessage.FromSystem("Imagine we have a SQL Server table called Employees. This table has three columns. First a column called Id which is a an integer auto increment primary key."),
                ChatMessage.FromSystem("First a column called Id which is an auto increment integer used as primary key."),
                ChatMessage.FromSystem("Second a column called Name with nvarchar type containing employee names."),
                ChatMessage.FromSystem("And third a column called Salary with decimal type"),
                ChatMessage.FromSystem("When asked to write a sql query just write the sql query and nothing else, this is very important"),
                ChatMessage.FromUser("Write a sql query that selects employees if their name has scott")
            },
            Model = GPT3.ObjectModels.Models.ChatGpt3_5Turbo,
        });

        if (!completionResult.Successful)
        {
            throw new Exception(completionResult.Error?.Message);
        }

        return completionResult.Choices.First().Message.Content;
    }

    public async Task<string> GetAsync2(string prompt)
    {
        var completionResult = await _openAIService.Completions.CreateCompletion(new CompletionCreateRequest
        {
            Prompt =
                @"Imagine we have a SQL Server table called Employees. This table has three columns. First a column called Id which is a an integer auto increment primary key
                First a column called Id which is an auto increment integer used as primary key
                Second a column called Name with nvarchar type containing employee names
                And third a column called Salary with decimal type
                Write a sql query that selects salary of all employees called scott",
            Model = GPT3.ObjectModels.Models.TextDavinciV3,
        });

        if (!completionResult.Successful)
        {
            throw new Exception(completionResult.Error?.Message);
        }

        return completionResult.Choices.First().Text;
    }
}
