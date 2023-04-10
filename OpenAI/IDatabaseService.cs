namespace OpenAI;

public interface IDatabaseService
{
    Task<List<string>> GetSchemaAsync();

    Task<List<dynamic>> GetAsync(string query);
}
