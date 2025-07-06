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

    public FunctionDefinition GetFunctionDefinition()
    {
        return new FunctionDefinition
        {
            Name = nameof(SqlListPlugin),
            Description = "A function that receives Microsoft SQL Server SQL query executes the query and puts the result in memory",
            Parameters = BinaryData.FromObjectAsJson(new
            {
                type = "object",
                properties = new Dictionary<string, object>()
                {
                    { nameof(Arguments.SqlQuery), new { type = "string", description = "The Microsoft SQL Server SQL query" } }
                },
                required = new[] { nameof(Arguments.SqlQuery) }
            })
        };
    }

    public Task<List<Message>?> BuildPromptAsync(object? _, ILogger logger)
    {
        var sqlServerService = new SqlServerService(logger);
        var schema = sqlServerService.GetSchema(_connectionString, _tableNames);

        if (schema is null)
        {
            return Task.FromResult((List<Message>?)null);
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

    public async Task<object?> GetResultAsync(string sqlQuery, ILogger logger)
    {
        var sqlServerService = new SqlServerService(logger);
        var data = await sqlServerService.QueryAsync(_connectionString, sqlQuery);

        return data;
    }

    private class Arguments
    {
        public string? SqlQuery { get; set; }
    }
}
