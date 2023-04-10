using Dapper;
using DatabaseSchemaReader.DataSchema;
using Microsoft.Data.SqlClient;

namespace OpenAI;

internal class SqlServerService : IDatabaseService
{
    private readonly string _connectionString;
    private readonly ILogger<SqlServerService> _logger;

    public SqlServerService(IConfiguration configuration, ILogger<SqlServerService> logger)
    {
        _connectionString = configuration.GetConnectionString("SqlServer") ?? throw new Exception("SqlServer connection string not found.");
        _logger = logger;
    }

    public Task<List<string>> GetSchemaAsync(string connectionString, List<string> tables)
    {
        var tablesSchema = GetSchema(connectionString, tables);

        var result = new List<string>
        {
            "Imagine we have a SQL Server database with these tables."
        };

        foreach (var table in tablesSchema)
        {
            result.Add($"A table called {table.Name} with {table.Columns.Count} columns");

            var prompts = GetPrompts(table);

            result.AddRange(prompts);
        }

        return Task.FromResult(result);
    }

    private static List<DatabaseTable> GetSchema(string connectionString, List<string> tables)
    {
        using var sqlConnection = new SqlConnection(connectionString);
        var dbReader = new DatabaseSchemaReader.DatabaseReader(sqlConnection);

        var schema = dbReader.ReadAll();

        return schema.Tables.Where(item => tables.Contains(item.Name)).ToList();
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

    public async Task<List<dynamic>> GetAsync(string query)
    {
        using var connection = new SqlConnection(_connectionString);

        try
        {
            var result = (await connection.QueryAsync(query)).ToList();

            return result;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Sql query failed: {Query}", query);
            return new();
        }
    }
}
