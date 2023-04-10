using Microsoft.AspNetCore.Mvc;
using OpenAI.Models;

namespace OpenAI.Controllers;

public class HomeController : Controller
{
    private readonly AIService _aIService;
    private readonly IDatabaseService _databaseService;

    public HomeController(AIService aIService, IDatabaseService databaseService)
    {
        _aIService = aIService;
        _databaseService = databaseService;
    }

    public IActionResult Index()
    {
        var model = new IndexViewModel();

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Index(string prompt)
    {
        var schema = await _databaseService.GetSchemaAsync();

        var sqlQuery = await _aIService.GetAsync(schema, prompt);

        sqlQuery = sqlQuery.Replace("```", "");

        var data = await _databaseService.GetAsync(sqlQuery);

        var model = new IndexViewModel(new() { prompt }, sqlQuery, data);

        return View(model);
    }
}
