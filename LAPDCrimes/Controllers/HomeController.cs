using LAPDCrimes.Models;
using LAPDCrimes.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LAPDCrimes.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private QueryService _queryService;
        public HomeController(ILogger<HomeController> logger,QueryService queryService)
        {
            _logger = logger;
            _queryService = queryService;
        }

        public async Task<IActionResult> Index()
        {
            return View();
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
}
