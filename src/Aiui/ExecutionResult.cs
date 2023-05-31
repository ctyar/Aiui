using System.Collections.Generic;

namespace Aiui;

public sealed class ExecutionResult
{
    public List<Message> ChatHistory { get; }

    public bool IsSuccessful { get; }

    public List<string>? Schema { get; }

    public string? Query { get; }

    public List<dynamic>? Rows { get; }

    internal ExecutionResult(List<Message> chatHistory)
    {
        ChatHistory = chatHistory;
        IsSuccessful = false;
    }

    internal ExecutionResult(List<Message> chatHistory, List<string> schema)
    {
        ChatHistory = chatHistory;
        IsSuccessful = false;
        Schema = schema;
    }

    internal ExecutionResult(List<Message> chatHistory, List<string> schema, string query, List<dynamic> rows)
    {
        ChatHistory = chatHistory;
        IsSuccessful = true;
        Schema = schema;
        Query = query;
        Rows = rows;
    }
}
