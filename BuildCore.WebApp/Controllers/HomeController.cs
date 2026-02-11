using System.Diagnostics;
using BuildCore.WebApp.Filters;
using BuildCore.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace BuildCore.WebApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    // Login olmadan erişilebilir
    public IActionResult Index()
    {
        return View();
    }

    [RequireLogin]
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
