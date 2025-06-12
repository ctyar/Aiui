using Aiui;
using Dapper;
using Microsoft.Data.SqlClient;

namespace AiuiWeb.RequestLog;

public class RequestLogService
{
    private readonly string _connectionString;
    private readonly ILogger<RequestLogService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RequestLogService(IConfiguration configuration, ILogger<RequestLogService> logger, IHttpContextAccessor httpContextAccessor)
    {
        _connectionString = configuration.GetConnectionString("SqlServerWriter") ?? throw new Exception("Missing SqlServerWriter connection string.");
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task SaveAsync(string? prompt, List<Message> chatHistory, string? response, CancellationToken cancellationToken)
    {
        var sessionId = _httpContextAccessor.HttpContext!.Session.Id;

        using var connection = new SqlConnection(_connectionString);

        try
        {
            var command = new CommandDefinition(
                "INSERT INTO [Requests] ([SessionId], [Prompt], [ChatHistory], [Response]) VALUES (@sessionId, @prompt, @chatHistory, @response)",
                new
                {
                    sessionId = sessionId,
                    prompt = prompt,
                    chatHistory = string.Join(",", chatHistory.Select(item => item.Content)),
                    response = response
                },
                cancellationToken: cancellationToken);

            await connection.ExecuteAsync(command);
        }
        catch (SqlException e)
        {
            _logger.LogWarning(e, "Request logging failed");
        }
    }
}
