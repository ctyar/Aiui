using Dapper;
using Microsoft.Data.SqlClient;

namespace OpenAI;

internal class SqlService : IDatabaseService
{
    private readonly string _connectionString;
    private readonly ILogger<SqlService> _logger;

    public SqlService(IConfiguration configuration, ILogger<SqlService> logger)
    {
        _connectionString = configuration.GetConnectionString("SqlServer") ?? throw new Exception("SqlServer connection string not found.");
        _logger = logger;
    }

    public Task<List<string>> GetSchemaAsync()
    {
        var result = new List<string>
        {
            "Imagine we have a SQL Server table called Products. This table has 10 columns",
            "Column 1 called ProductID which is a an integer auto increment primary key",
            "Column 2 called ProductName is non nullable nvarchar which has the name of the product",
            "Column 3 called SupplierID is a nullable foreign key to table Suppliers which has the supplier's information",
            "Column 4 called CategoryID is a nullable foreign key to table Categories which has the products category information",
            "Column 5 called QuantityPerUnit is a nullable nvarchar which has the product's quantity for each unit",
            "Column 6 called UnitPrice is a nullable money which has the product's price",
            "Column 7 called UnitsInStock is a nullable smallint which shows how many units of the product we have in stock",
            "Column 8 called UnitsOnOrder is a nullable smallint which shows how many units of the product we ordered right now",
            "Column 9 called ReorderLevel is a nullable smallint",
            "Column 10 called Discontinued is a non nullable bit which shows if the product is discontinued or not or if it is inactive or active",
        };

        return Task.FromResult(result);
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
