namespace OpenAI;

public interface IBotService
{
    Task<ExecutionResult> ExecutePromptAsync(string connectionString, List<string> tableNames, string prompt);
}
