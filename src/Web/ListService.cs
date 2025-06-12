using Aiui;
using AiuiWeb.RequestLog;
using OpenTelemetry.Trace;

namespace AiuiWeb;

public class ListService
{
    private readonly BotService _botService;
    private readonly RequestLogService _requestLogService;
    private readonly Tracer _tracer;

    public ListService(BotService botService, RequestLogService requestLogService, Tracer tracer)
    {
        _botService = botService;
        _requestLogService = requestLogService;
        _tracer = tracer;
    }

    public async Task<ExecutionResult> GetAsync(string prompt, List<Message> chatHistory, CancellationToken cancellationToken)
    {
        ExecutionResult executionResult;
        using (_tracer.Step(nameof(BotService.ExecutePromptAsync)))
        {
            executionResult = await _botService.ExecutePromptAsync(prompt, chatHistory, null);
        }

        using (_tracer.Step(nameof(RequestLogService.SaveAsync)))
        {
            await _requestLogService.SaveAsync(prompt, executionResult.ChatHistory, executionResult.Response, cancellationToken);
        }

        return executionResult;
    }
}
