using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aiui;

public sealed class BotService
{
    private readonly string _openAIApiKey;
    private SqlServerService? _databaseService;
    private OpenAiService? _openAiService;

    private SqlServerService DatabaseService
    {
        get
        {
            _databaseService ??= new SqlServerService();

            return _databaseService;
        }
    }

    private OpenAiService OpenAiService
    {
        get
        {
            _openAiService ??= new OpenAiService(_openAIApiKey);

            return _openAiService;
        }
    }

    public BotService(string openAIApiKey)
    {
        _openAIApiKey = openAIApiKey;
    }

    public async Task<ExecutionResult> ExecutePromptAsync(string connectionString, List<string> tableNames, string prompt)
    {
        var schema = DatabaseService.GetSchema(connectionString, tableNames);

        if (schema is null)
        {
            return new ExecutionResult();
        }

        var sqlQuery = await OpenAiService.GetAsync(schema, prompt);

        if (sqlQuery is null)
        {
            return new ExecutionResult(schema);
        }

        sqlQuery = sqlQuery.Replace("```", "");

        var data = await DatabaseService.GetAsync(connectionString, sqlQuery);

        if (data is null)
        {
            return new ExecutionResult(schema, sqlQuery);
        }

        return new ExecutionResult(schema, sqlQuery, data);
    }
}
