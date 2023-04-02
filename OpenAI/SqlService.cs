using Dapper;
using Microsoft.Data.SqlClient;

namespace OpenAI;

public class SqlService
{
    private readonly string _connectionString;
    private readonly ILogger<SqlService> _logger;

    public SqlService(IConfiguration configuration, ILogger<SqlService> logger)
    {
        _connectionString = configuration.GetConnectionString("SqlServer") ?? throw new Exception("SqlServer connection string not found.");
        _logger = logger;
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
