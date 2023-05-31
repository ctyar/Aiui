using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.AI.OpenAI;

namespace Aiui;

public sealed class BotService
{
    private readonly SqlServerService _sqlServerService;

    public BotService(SqlServerService sqlServerService)
    {
        _sqlServerService = sqlServerService;
    }

    public async Task<ExecutionResult> ExecutePromptAsync(string connectionString, OpenAIClient openAIClient, List<string> tableNames, string prompt,
        List<Message> chatHistory)
    {
        ArgumentNullException.ThrowIfNull(connectionString);
        ArgumentNullException.ThrowIfNull(openAIClient);
        ArgumentNullException.ThrowIfNull(tableNames);
        ArgumentNullException.ThrowIfNull(prompt);
        ArgumentNullException.ThrowIfNull(chatHistory);

        var newHistory = GetNewHistory(prompt, chatHistory);

        var schema = _sqlServerService.GetSchema(connectionString, tableNames);

        if (schema is null)
        {
            return new ExecutionResult(newHistory);
        }

        var openAiService = new OpenAIService(openAIClient);
        var sqlQuery = await openAiService.GetAsync(schema, prompt, chatHistory);

        if (sqlQuery is null)
        {
            return new ExecutionResult(newHistory, schema);
        }

        sqlQuery = CleanQuery(sqlQuery);

        var data = await _sqlServerService.QueryAsync(connectionString, sqlQuery);

        if (data is null)
        {
            // Probably just a normal command response
            newHistory.Add(new Message
            {
                Type = MessageType.System,
                Content = sqlQuery
            });

            return new ExecutionResult(newHistory, schema);
        }
        else
        {
            newHistory.Add(new Message
            {
                Type = MessageType.Info,
                Content = "Done"
            });
        }

        return new ExecutionResult(newHistory, schema, sqlQuery, data);
    }

    private static string CleanQuery(string query)
    {
        // Remove ``` anywhere in the query
        query = query.Replace("```sql", "");
        query = query.Replace("```", "");

        // Remove everything before the first select
        var index = query.IndexOf("select", 0, StringComparison.OrdinalIgnoreCase);

        if (index <= 1)
        {
            return query;
        }

        return query.Substring(index);
    }

    private static List<Message> GetNewHistory(string prompt, List<Message> chatHistory)
    {
        var newChatHistory = chatHistory.ToList();

        newChatHistory.Add(new Message
        {
            Type = MessageType.User,
            Content = prompt
        });

        return newChatHistory;
    }
}
