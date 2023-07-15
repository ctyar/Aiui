# AI as User Interface

[![Build Status](https://ctyar.visualstudio.com/Aiui/_apis/build/status%2Fctyar.Aiui?branchName=main)](https://ctyar.visualstudio.com/Aiui/_build/latest?definitionId=8&branchName=main)
[![Aiui](https://img.shields.io/nuget/v/Aiui.svg)](https://www.nuget.org/packages/Aiui/)

[Demo video](https://github.com/ctyar/Aiui/assets/1432648/b97d3bd2-f5a4-4c08-a17c-80904411cb07)

This project aims to facilitate the creation of a new type of user interface for line of business applications.
It will allow users to query data in their application using natural language and lower the barrier of accessing and analyzing data.
This is similar to how most applications let users export data as excel files to do further analysis on their own.

## [Live demo: https://aiui.azurewebsites.net](https://aiui.azurewebsites.net/)

## Usage
1. Install the [NuGet package](https://www.nuget.org/packages/Aiui)

2. In the `Program.cs`, register the Aiui.
```csharp
using Aiui;
```
```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAiui();
```

3. Pass your SQL Server connection string and list of tables you want to expose, with user's current prompt and chat history to the `BotService`.
```csharp
[HttpPost("")]
public async Task<IActionResult> Index(string prompt, List<string> chatHistory)
{
    var tableNames = new List<string> { "Products", "Categories" };

    var plugin = new SqlListPlugin(_connectionString, tableNames);
    var executionResult = await _botService.ExecutePromptAsync(plugin, new OpenAIClient(_openAIApiKey), prompt, chatHistory, null);

    return View(executionResult);
}
```

4. You can optionally use the Chart.js plugin to draw charts.
```csharp
[HttpPost("")]
public async Task<IActionResult> Chart(string prompt, List<string> chatHistory, List<dynamic> rows)
{
    var plugin = new ChartJsPlugin();
    var executionResult = await _botService.ExecutePromptAsync(plugin, new OpenAIClient(_openAIApiKey), prompt, chatHistory, rows);

    return View(executionResult);
}
```

## Build
[Install](https://get.dot.net) the [required](global.json) .NET SDK.

Run:
```
$ dotnet build
```
