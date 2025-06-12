using Aiui;
using AiuiWeb.Models;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Trace;

namespace AiuiWeb.Controllers;

[Route("")]
public class HomeController : Controller
{
    private readonly ListService _listService;
    private readonly DataStorageService _dataStorageService;
    private readonly GraphService _graphService;
    private readonly Tracer _tracer;

    public HomeController(ListService listService, DataStorageService dataStorageService, GraphService graphService, Tracer tracer)
    {
        _listService = listService;
        _dataStorageService = dataStorageService;
        _graphService = graphService;
        _tracer = tracer;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        HttpContext.Session.Set("-", new byte[] { 1 });

        var rows = _dataStorageService.Get();

        var model = new IndexViewModel
        {
            IsFirst = true,
            Response = null,
            Result = rows,
            Context = null
        };

        return View(model);
    }

    [HttpGet("list")]
    public IActionResult GetList()
    {
        var rows = _dataStorageService.Get();

        var model = new IndexViewModel
        {
            IsFirst = true,
            Response = null,
            Result = rows,
            Context = null
        };

        return View("Index", model);
    }

    [HttpPost("list")]
    public async Task<IActionResult> PostList(string? prompt, List<Message> chatHistory, CancellationToken cancellationToken)
    {
        using var _ = _tracer.Step(nameof(HomeController.PostList));

        if (prompt is null)
        {
            return View(new IndexViewModel
            {
                IsFirst = true,
                Response = null,
                Result = null,
                Context = null
            });
        }

        var executionResult = await _listService.GetAsync(prompt, chatHistory, cancellationToken);

        _dataStorageService.Set((List<dynamic>?)executionResult.Result);

        var model = new IndexViewModel
        {
            IsFirst = false,
            Response = executionResult.Response,
            Result = executionResult.Result,
            Context = null,
            ChatHistory = executionResult.ChatHistory
        };

        return View("Index", model);
    }

    [HttpPost("graph")]
    public async Task<IActionResult> PostGraph(string? prompt, List<Message> chatHistory, CancellationToken cancellationToken)
    {
        using var _ = _tracer.Step(nameof(HomeController.PostGraph));

        if (prompt is null)
        {
            return Redirect("~/");
        }

        var rows = _dataStorageService.Get();

        if (rows is null)
        {
            return Redirect("~/");
        }

        var executionResult = await _graphService.GetAsync(prompt, chatHistory, rows, cancellationToken);

        var model = new IndexViewModel
        {
            IsFirst = false,
            Response = executionResult.Response,
            Result = executionResult.Result,
            Context = rows,
            ChatHistory = executionResult.ChatHistory
        };

        return View("Graph", model);
    }

    [HttpGet("graph")]
    public IActionResult GetGraph()
    {
        var rows = _dataStorageService.Get();

        var model = new IndexViewModel
        {
            IsFirst = true,
            Response = null,
            Result = null,
            Context = rows,
        };

        return View("Graph", model);
    }

    [Route("error")]
    public IActionResult HandleError()
    {
        return Redirect("~/");
    }
}
