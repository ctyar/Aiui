using Aiui;
using Microsoft.AspNetCore.Mvc;
using OpenAI.Models;

namespace OpenAI.Controllers;

public class HomeController : Controller
{
    private readonly IConfiguration _configuration;

    public HomeController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IActionResult Index()
    {
        var model = new IndexViewModel();

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Index(string prompt)
    {
        var connectionString = _configuration.GetConnectionString("SqlServer") ?? throw new Exception("Missing SQL Server connection string.");
        var tableNames = new List<string> { "Products", "Categories" };

        var botService = new BotService(connectionString);
        var executionResult = await botService.ExecutePromptAsync(connectionString, tableNames, prompt);

        var model = new IndexViewModel(new() { prompt }, executionResult);

        return View(model);
    }
}
