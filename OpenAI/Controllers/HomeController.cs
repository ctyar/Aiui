using Microsoft.AspNetCore.Mvc;
using OpenAI.Models;

namespace OpenAI.Controllers;

public class HomeController : Controller
{
    private readonly IBotService _botService;
    private readonly IConfiguration _configuration;

    public HomeController(IBotService botService, IConfiguration configuration)
    {
        _botService = botService;
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

        var executionResult = await _botService.ExecutePromptAsync(connectionString, tableNames, prompt);

        var model = new IndexViewModel(new() { prompt }, executionResult);

        return View(model);
    }
}
