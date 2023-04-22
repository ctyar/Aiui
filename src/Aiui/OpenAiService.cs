﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenAI.GPT3;
using OpenAI.GPT3.Managers;
using OpenAI.GPT3.ObjectModels.RequestModels;

namespace Aiui;

public sealed class OpenAiService
{
    private readonly OpenAIService _openAIService;

    public OpenAiService(string _openAIApiKey)
    {
        _openAIService = new OpenAIService(new OpenAiOptions()
        {
            ApiKey = _openAIApiKey
        });
    }

    public async Task<string?> GetAsync(List<string> schema, string prompt)
    {
        var messages = schema.Select(ChatMessage.FromSystem).ToList();

        messages.Add(ChatMessage.FromSystem("When instructed to list or show or create a report create a SQL query with the above knowledge instead"));
        messages.Add(ChatMessage.FromSystem("When creating a SQL query just write the SQL query itself and nothing else, this is very important"));

        messages.Add(ChatMessage.FromUser(prompt ?? "Write a sql query that selects all the active products"));

        var completionResult = await _openAIService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
        {
            Messages = messages,
            Model = OpenAI.GPT3.ObjectModels.Models.ChatGpt3_5Turbo,
        });

        if (!completionResult.Successful)
        {
            return null;
        }

        return completionResult.Choices.First().Message.Content;
    }
}