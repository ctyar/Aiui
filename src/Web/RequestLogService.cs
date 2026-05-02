using System.Globalization;
using System.Text;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.AI;

namespace Web;

public class RequestLogService
{
    private readonly string _connectionString;
    private readonly ILogger<RequestLogService> _logger;

    public RequestLogService(IConfiguration configuration, ILogger<RequestLogService> logger)
    {
        _connectionString = configuration.GetConnectionString("SqlServerWriter") ?? throw new InvalidOperationException("Missing SqlServerWriter connection string.");
        _logger = logger;
    }

    public async Task SaveAsync(string? prompt, List<ChatMessage> messages, Guid conversionId)
    {
        using var connection = new SqlConnection(_connectionString);

        try
        {
            var command = new CommandDefinition(
                "INSERT INTO [Requests] ([SessionId], [Prompt], [ChatHistory]) VALUES (@sessionId, @prompt, @chatHistory)",
                new
                {
                    sessionId = conversionId.ToString(),
                    prompt = prompt,
                    chatHistory = GetLogMessage(messages),
                });

            await connection.ExecuteAsync(command);
        }
        catch (SqlException e)
        {
            _logger.RequestFailed(e);
        }
    }

    private static string GetLogMessage(List<ChatMessage> messages)
    {
        var result = new StringBuilder();

        foreach (var message in messages.Where(i => i.Role != ChatRole.System))
        {
            if (!string.IsNullOrEmpty(message.Text))
            {
                result.Append(CultureInfo.InvariantCulture, $"{message.Role}: {message.Text}\r\n");

                continue;
            }

            if (message.Contents.Count > 0 && message.Contents[0] is FunctionCallContent functionCallContent)
            {
                result.Append(CultureInfo.InvariantCulture, $"{message.Role}: Execute {functionCallContent.Name}() {string.Join(",", functionCallContent.Arguments?.Values ?? [])}\r\n");
            }
        }

        return result.ToString();
    }
}
