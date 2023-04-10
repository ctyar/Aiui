namespace OpenAI.Models;

public class IndexViewModel
{
    public List<string> Messages { get; }

    public ExecutionResult? ExecutionResult { get; }

    public IndexViewModel()
    {
        Messages = new();
        ExecutionResult = null;
    }

    public IndexViewModel(List<string> messages, ExecutionResult executionResult)
    {
        Messages = messages;
        ExecutionResult = executionResult;
    }
}
