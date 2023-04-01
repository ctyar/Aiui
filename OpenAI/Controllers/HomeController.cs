using Microsoft.AspNetCore.Mvc;
using OpenAI.Models;

namespace OpenAI.Controllers;

public class HomeController : Controller
{
    private readonly AIService _aIService;
    private readonly EmployeeService _employeeService;

    public HomeController(AIService aIService, EmployeeService employeeService)
    {
        _aIService = aIService;
        _employeeService = employeeService;
    }

    public IActionResult Index()
    {
        var model = new IndexViewModel();

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Index(string prompt)
    {
        var sqlQuery = await _aIService.GetAsync(prompt);

        var employees = await _employeeService.GetAsync(sqlQuery);

        var model = new IndexViewModel(new() { prompt }, sqlQuery, employees);

        return View(model);
    }
}
