using System.Collections.Generic;

namespace Aiui;

public sealed class ExecutionResult
{
    public List<string> ChatHistory { get; }

    public bool IsSuccessful { get; }

    public List<string>? Schema { get; }

    public string? Query { get; }

    public List<dynamic>? Rows { get; }

    internal ExecutionResult(List<string> chatHistory)
    {
        ChatHistory = chatHistory;
        IsSuccessful = false;
    }

    internal ExecutionResult(List<string> chatHistory, List<string> schema)
    {
        ChatHistory = chatHistory;
        IsSuccessful = false;
        Schema = schema;
    }

    internal ExecutionResult(List<string> chatHistory, List<string> schema, string query, List<dynamic> rows)
    {
        ChatHistory = chatHistory;
        IsSuccessful = true;
        Schema = schema;
        Query = query;
        Rows = rows;
    }
}
