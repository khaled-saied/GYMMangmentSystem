using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GYMMangmentSystem.Models;
using GymMangment.BLL.Services.Interfaces;
using System.Threading.Tasks;

namespace GYMMangmentSystem.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IAnalyticService _service;

    public HomeController(ILogger<HomeController> logger,IAnalyticService service)
    {
        _logger = logger;
        _service = service;
    }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var data =await _service.GetAnalyticsAsync(ct);
        return View(data);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
