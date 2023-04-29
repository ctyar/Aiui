using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DatabaseSchemaReader.DataSchema;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace Aiui;

public sealed class SqlServerService
{
    private readonly ILogger<SqlServerService> _logger;

    public SqlServerService(ILogger<SqlServerService> logger)
    {
        _logger = logger;
    }

    public List<string>? GetSchema(string connectionString, List<string> tables)
    {
        var tablesSchema = GetSchemaCore(connectionString, tables);

        if (tablesSchema is null)
        {
            return null;
        }

        var result = new List<string>
        {
            "Imagine we have a Microsoft SQL Server database with these tables."
        };

        foreach (var table in tablesSchema)
        {
            result.Add($"A table called {table.Name} with {table.Columns.Count} columns");

            var prompts = GetPrompts(table);

            result.AddRange(prompts);
        }

        return result;
    }

    private List<DatabaseTable>? GetSchemaCore(string connectionString, List<string> tables)
    {
        using var sqlConnection = new SqlConnection(connectionString);

        try
        {
            var dbReader = new DatabaseSchemaReader.DatabaseReader(sqlConnection);

            var schema = dbReader.ReadAll();

            return schema.Tables.Where(item => tables.Contains(item.Name)).ToList();
        }
        catch (SqlException e)
        {
            _logger.LogError(e, "Getting schema failed");
            return null;
        }
    }

    private static List<string> GetPrompts(DatabaseTable databaseTable)
    {
        var result = new List<string>();
        var i = 1;

        foreach (var column in databaseTable.Columns)
        {
            var nullability = column.Nullable ? "nullable" : "non nullable";
            var primaryKey = column.IsPrimaryKey ? "primary key" : string.Empty;
            var foreignKey = column.IsForeignKey ? "foreign key" : string.Empty;

            result.Add($"Column {i} called {column.Name} which is a an {nullability} {column.DataType.TypeName} {primaryKey} {foreignKey}");
            i++;
        }

        return result;
    }

    public async Task<List<dynamic>?> QueryAsync(string connectionString, string query)
    {
        using var connection = new SqlConnection(connectionString);

        try
        {
            var result = (await connection.QueryAsync(query)).ToList();

            return result;
        }
        catch (SqlException e)
        {
            _logger.LogError(e, "Executing query failed");
            return null;
        }
    }
}
