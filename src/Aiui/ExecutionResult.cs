using System.Collections.Generic;

namespace Aiui;

public sealed class ExecutionResult
{
    public bool IsSuccessful { get; }

    public List<string>? Schema { get; }

    public string? Query { get; }

    public List<dynamic>? Rows { get; }

    internal ExecutionResult()
    {
        IsSuccessful = false;
    }

    internal ExecutionResult(List<string> schema)
    {
        IsSuccessful = false;
        Schema = schema;
    }

    internal ExecutionResult(List<string> schema, string query)
    {
        IsSuccessful = false;
        Schema = schema;
        Query = query;
    }

    internal ExecutionResult(List<string> schema, string query, List<dynamic> rows)
    {
        IsSuccessful = true;
        Schema = schema;
        Query = query;
        Rows = rows;
    }
}
