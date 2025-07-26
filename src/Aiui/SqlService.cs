using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DatabaseSchemaReader.DataSchema;
using Microsoft.Data.SqlClient;

namespace Aiui;

internal sealed class SqlServerService
{
    public static string? GetSchema(string connectionString, List<string> tables)
    {
        var tablesSchema = GetSchemaCore(connectionString, tables);

        if (tablesSchema is null)
        {
            return null;
        }

        var prompt = new StringBuilder();
        prompt.Append("We have a Microsoft SQL Server database with the following tables:\r\n");

        // TODO: Test valid SQL instead of this
        foreach (var (index, table) in tablesSchema.Index())
        {
            prompt.Append($"{index}. [{table.Name}] with {table.Columns.Count} columns.\r\n");

            AddColumns(table, prompt);
        }

        return prompt.ToString();
    }

    public static async Task<List<dynamic>?> QueryAsync(string connectionString, string query)
    {
        using var connection = new SqlConnection(connectionString);

        try
        {
            var result = (await connection.QueryAsync(query)).ToList();

            return result;
        }
        catch (SqlException)
        {
            return null;
        }
    }

    private static List<DatabaseTable>? GetSchemaCore(string connectionString, List<string> tables)
    {
        using var sqlConnection = new SqlConnection(connectionString);

        try
        {
            var dbReader = new DatabaseSchemaReader.DatabaseReader(sqlConnection);

            var schema = dbReader.ReadAll();

            return schema.Tables.Where(item => tables.Contains(item.Name)).ToList();
        }
        catch (SqlException)
        {
            return null;
        }
    }

    private static void AddColumns(DatabaseTable databaseTable, StringBuilder prompt)
    {
        foreach (var (index, column) in databaseTable.Columns.Index())
        {
            var nullability = column.Nullable ? "nullable" : "non nullable";

            var keyType = string.Empty;
            if (column.IsPrimaryKey)
            {
                keyType = " primary key";
            }
            else if (column.IsForeignKey)
            {
                keyType = " foreign key";
            }

            prompt.Append($"    {index}. [{column.Name}] which is a {nullability} {column.DataType.TypeName}{keyType}.\r\n");
        }
    }
}
