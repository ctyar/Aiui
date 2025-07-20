using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

    public Task<List<Message>> GetRootPromptAsync()
    {
        var schema = _sqlServerService.GetSchema(_connectionString, _tableNames);

        if (schema is null)
        {
            return Task.FromResult<List<Message>>([]);
        }

        var result = new List<Message>();
        foreach (var tableSchema in schema)
        {
            result.Add(new Message
            {
                Type = MessageType.AI,
                Content = tableSchema
            });
        }

        return Task.FromResult(result)!;
    }

    public Task<List<Message>> GetContextPromptAsync(object? context)
    {
        return Task.FromResult<List<Message>>([]);
    }

    public async Task<object?> ExecuteAsync(string sqlQuery)
    {
        var data = await _sqlServerService.QueryAsync(_connectionString, sqlQuery);

        return data;
    }
}
