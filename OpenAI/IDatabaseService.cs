namespace OpenAI;

public interface IDatabaseService
{
    Task<List<string>> GetSchemaAsync(string connectionString, List<string> tables);

    Task<List<dynamic>> GetAsync(string query);
}
