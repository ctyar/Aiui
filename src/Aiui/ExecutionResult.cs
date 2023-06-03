using System.Collections.Generic;

namespace Aiui;

public sealed class ExecutionResult
{
    public List<Message> ChatHistory { get; }

    public bool IsSuccessful { get; }

    public string? AiResponse { get; }

    public List<dynamic>? Data { get; }

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

    internal ExecutionResult(List<Message> chatHistory, string? aiResponse, List<dynamic> data)
    {
        IsSuccessful = true;
        ChatHistory = chatHistory;
        AiResponse = aiResponse;
        Data = data;
    }
}
