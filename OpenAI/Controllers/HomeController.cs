using Microsoft.AspNetCore.Mvc;

namespace OpenAI.Controllers;

public class HomeController : Controller
{
    private readonly AIService _aIService;

    public HomeController(AIService aIService)
    {
        _aIService = aIService;
    }

    public IActionResult Index()
    {
        ViewBag.Data = new List<string>();

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Index(string query)
    {
        var response = await _aIService.GetAsync(query);

        ViewBag.Data = new List<string>
        {
            query,
            response
        };

        return View();
    }
}
