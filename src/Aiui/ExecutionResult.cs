using System.Collections.Generic;

namespace Aiui;

public sealed class ExecutionResult
{
    public List<Message> ChatHistory { get; }

    public bool IsSuccessful { get; }

    public string? AiResponse { get; }

    public object? Result { get; }

    internal ExecutionResult(List<Message> chatHistory)
    {
        IsSuccessful = false;
        ChatHistory = chatHistory;
    }

    internal ExecutionResult(List<Message> chatHistory, string? aiResponse)
    {
        IsSuccessful = false;
        ChatHistory = chatHistory;
        AiResponse = aiResponse;
    }

    internal ExecutionResult(List<Message> chatHistory, string? aiResponse, object? result)
    {
        IsSuccessful = true;
        ChatHistory = chatHistory;
        AiResponse = aiResponse;
        Result = result;
    }
}
