namespace OpenAI;

public class BotService : IBotService
{
    private readonly IDatabaseService _databaseService;
    private readonly OpenAiService _openAiService;

    public BotService(IDatabaseService databaseService, OpenAiService openAiService)
    {
        _databaseService = databaseService;
        _openAiService = openAiService;
    }

    public async Task<ExecutionResult> ExecutePromptAsync(string connectionString, List<string> tableNames, string prompt)
    {
        var schema = await _databaseService.GetSchemaAsync(connectionString, tableNames);

        var sqlQuery = await _openAiService.GetAsync(schema, prompt);

        sqlQuery = sqlQuery.Replace("```", "");

        var data = await _databaseService.GetAsync(sqlQuery);

        return new ExecutionResult(true, schema, sqlQuery, data);
    }
}
