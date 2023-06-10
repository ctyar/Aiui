using System.Collections.Generic;

namespace Aiui;

public sealed class ExecutionResult
{
    public List<Message> ChatHistory { get; }

    public bool IsSuccessful { get; }

    public string? Response { get; }

    public object? Result { get; }

    internal ExecutionResult(List<Message> chatHistory)
    {
        IsSuccessful = false;
        ChatHistory = chatHistory;
    }

    internal ExecutionResult(List<Message> chatHistory, string? response)
    {
        IsSuccessful = false;
        ChatHistory = chatHistory;
        Response = response;
    }

    internal ExecutionResult(List<Message> chatHistory, string? response, object? result)
    {
        IsSuccessful = true;
        ChatHistory = chatHistory;
        Response = response;
        Result = result;
    }
}
