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
                ChatMessage.FromSystem("Imagine we have a SQL Server table called Products. This table has 10 columns"),
                ChatMessage.FromSystem("Column 1 called ProductID which is a an integer auto increment primary key"),
                ChatMessage.FromSystem("Column 2 called ProductName is non nullable nvarchar which has the name of the product"),
                ChatMessage.FromSystem("Column 3 called SupplierID is a nullable foreign key to table Suppliers which has the supplier's information"),
                ChatMessage.FromSystem("Column 4 called CategoryID is a nullable foreign key to table Categories which has the products category information"),
                ChatMessage.FromSystem("Column 5 called QuantityPerUnit is a nullable nvarchar which has the product's quantity for each unit"),
                ChatMessage.FromSystem("Column 6 called UnitPrice is a nullable money which has the product's price"),
                ChatMessage.FromSystem("Column 7 called UnitsInStock is a nullable smallint which shows how many units of the product we have in stock"),
                ChatMessage.FromSystem("Column 8 called UnitsOnOrder is a nullable smallint which shows how many units of the product we ordered right now"),
                ChatMessage.FromSystem("Column 9 called ReorderLevel is a nullable smallint"),
                ChatMessage.FromSystem("Column 10 called Discontinued is a non nullable bit which shows if the product is discontinued or not"),

                ChatMessage.FromSystem("When asked to write a sql query just write the sql query and nothing else, this is very important"),

                ChatMessage.FromUser("Write a sql query that selects all the active products")
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
