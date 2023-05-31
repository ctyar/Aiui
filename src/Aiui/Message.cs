namespace Aiui;

public sealed class Message
{
    public MessageType Type { get; set; }

    public string Content { get; set; } = null!;
}
