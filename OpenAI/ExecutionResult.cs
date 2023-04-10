namespace OpenAI;

public class ExecutionResult
{
    public bool IsSuccessful { get; }

    public List<string>? Schema { get; }

    public string? Query { get; }

    public List<dynamic> Rows { get; }

    public ExecutionResult(bool isSuccessful, List<string>? schema, string? query, List<dynamic> rows)
    {
        IsSuccessful = isSuccessful;
        Schema = schema;
        Query = query;
        Rows = rows;
    }
}
