using Microsoft.AspNetCore.Mvc;

namespace OpenAI.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Index(string query)
    {
        ViewBag.Data = "Hello!<br />World";

        return View();
    }
}
