using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aiui;

public sealed class BotService
{
    private readonly SqlServerService _sqlServerService;

    public BotService(SqlServerService sqlServerService)
    {
        _sqlServerService = sqlServerService;
    }

    public async Task<ExecutionResult> ExecutePromptAsync(string connectionString, string openAIApiKey, List<string> tableNames, string prompt)
    {
        var schema = _sqlServerService.GetSchema(connectionString, tableNames);

        if (schema is null)
        {
            return new ExecutionResult();
        }

        var openAiSevce = new OpenAiService(openAIApiKey);
        var sqlQuery = await openAiSevce.GetAsync(schema, prompt);

        if (sqlQuery is null)
        {
            return new ExecutionResult(schema);
        }

        sqlQuery = CleanQuery(sqlQuery);

        var data = await _sqlServerService.QueryAsync(connectionString, sqlQuery);

        if (data is null)
        {
            return new ExecutionResult(schema, sqlQuery);
        }

        return new ExecutionResult(schema, sqlQuery, data);
    }

    private static string CleanQuery(string query)
    {
        // Remove ``` anywhere in the query
        query = query.Replace("```", "");

        // Remove everything before the first select
        var index = query.IndexOf("select", 0, System.StringComparison.OrdinalIgnoreCase);

        if (index <= 1)
        {
            return query;
        }

        return query.Substring(index);
    }
}
