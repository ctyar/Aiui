using Aiui;

namespace AiuiWeb.Models;

public class IndexViewModel
{
    public required bool IsFirst { get; init; }

    public List<Message> ChatHistory { get; init; } = new();

    public required string? Response { get; init; }

    public required object? Result { get; init; }

    public required object? Context { get; init; }
}
