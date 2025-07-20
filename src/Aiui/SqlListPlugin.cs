using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.AI;

namespace Aiui;

public sealed class SqlListPlugin : IPlugin
{
    private readonly string _connectionString;
    private readonly List<string> _tableNames;
    private readonly SqlServerService _sqlServerService;

    public SqlListPlugin(string connectionString, List<string> tableNames)
    {
        ArgumentNullException.ThrowIfNull(connectionString);
        ArgumentNullException.ThrowIfNull(tableNames);

        _connectionString = connectionString;
        _tableNames = tableNames;
        _sqlServerService = new SqlServerService();
    }

    public Task<ChatMessage?> GetRootPromptAsync()
    {
        var schema = _sqlServerService.GetSchema(_connectionString, _tableNames);

        if (schema is null)
        {
            return Task.FromResult<ChatMessage?>(null);
        }

        var textContents = new List<AIContent>();

        foreach (var tableSchema in schema)
        {
            textContents.Add(new TextContent(tableSchema));
        }

        return Task.FromResult<ChatMessage?>(new ChatMessage
        {
            Role = ChatRole.System,
            Contents = textContents
        });
    }

    public Task<ChatMessage?> GetContextPromptAsync(object? context)
    {
        return Task.FromResult<ChatMessage?>(null);
    }

    public async Task<object?> ExecuteAsync(string sqlQuery)
    {
        var data = await _sqlServerService.QueryAsync(_connectionString, sqlQuery);

        return data;
    }
}
