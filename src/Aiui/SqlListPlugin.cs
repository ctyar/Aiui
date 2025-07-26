using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.AI;

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

    public Task<ChatMessage?> GetRootPromptAsync()
    {
        var schema = SqlServerService.GetSchema(_connectionString, _tableNames);

        if (schema is null)
        {
            return Task.FromResult<ChatMessage?>(null);
        }

        return Task.FromResult<ChatMessage?>(new ChatMessage
        {
            Role = ChatRole.System,
            Contents = [new TextContent(schema)]
        });
    }

    public Task<string?> GetContextPromptAsync(object? context)
    {
        return Task.FromResult<string?>(null);
    }

    public async Task<object?> ExecuteAsync(string sqlQuery)
    {
        var data = await SqlServerService.QueryAsync(_connectionString, sqlQuery);

        return data;
    }
}
