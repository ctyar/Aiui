using Dapper;
using Microsoft.Data.SqlClient;

namespace OpenAI;

public class EmployeeService
{
    private readonly string _connectionString;
    private readonly ILogger<EmployeeService> _logger;

    public EmployeeService(IConfiguration configuration, ILogger<EmployeeService> logger)
    {
        _connectionString = configuration.GetConnectionString("SqlServer") ?? throw new Exception("SqlServer connection string not found.");
        _logger = logger;
    }

    public async Task<List<Employee>> GetAsync(string query)
    {
        using var connection = new SqlConnection(_connectionString);

        try
        {
            var result = (await connection.QueryAsync<Employee>(query)).ToList();

            return result;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Sql query failed: {Query}", query);
            return new();
        }
    }
}

public class Employee
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public decimal Salary { get; set; }
}
