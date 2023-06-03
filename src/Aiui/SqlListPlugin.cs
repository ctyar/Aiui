using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Logging;

namespace Aiui;

public sealed class SqlListPlugin : IPlugin
{
    private readonly string _connectionString;
    private readonly List<string> _tableNames;

    public SqlListPlugin(string connectionString, List<string> tableNames)
    {
        ArgumentNullException.ThrowIfNull(connectionString);
        ArgumentNullException.ThrowIfNull(tableNames);

        _connectionString = connectionString;
        _tableNames = tableNames;
    }

    public Task<List<ChatMessage>?> BuildPromptAsync(string prompt, object? context, ILogger logger)
    {
        var sqlServerService = new SqlServerService(logger);
        var schema = sqlServerService.GetSchema(_connectionString, _tableNames);

        if (schema is null)
        {
            return Task.FromResult((List<ChatMessage>?)null);
        }

        var result = new List<ChatMessage>();
        foreach (var tableSchema in schema)
        {
            result.Add(new ChatMessage(ChatRole.System, tableSchema));
        }

        result.Add(new ChatMessage(ChatRole.System,
            "When instructed to list or show or create a report create a SQL query with the above knowledge instead"));

        result.Add(new ChatMessage(ChatRole.System,
            "When creating a SQL query you must be brief and no explanation just write the SQL query itself and nothing else, this is very important"));

        result.Add(new ChatMessage(ChatRole.User, $"{prompt}, no explanation"));

        return Task.FromResult(result)!;
    }

    public async Task<object?> GetResultAsync(string aiResponse, ILogger logger)
    {
        var sqlQuery = CleanQuery(aiResponse);

        var sqlServerService = new SqlServerService(logger);
        var data = await sqlServerService.QueryAsync(_connectionString, sqlQuery);

        return data;
    }

    private static string CleanQuery(string query)
    {
        // Remove ``` anywhere in the query
        query = query.Replace("```sql", "");
        query = query.Replace("```", "");

        // Remove everything before the first select
        var index = query.IndexOf("select", 0, StringComparison.OrdinalIgnoreCase);

        if (index <= 1)
        {
            return query;
        }

        return query.Substring(index);
    }
}
