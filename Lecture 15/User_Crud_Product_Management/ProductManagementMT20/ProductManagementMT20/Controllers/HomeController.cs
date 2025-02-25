using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductManagementMT20.Models;
using System.Diagnostics;

namespace ProductManagementMT20.Controllers
{
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

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AdminOnlyPage()
        {
            return View();
        }

        [Authorize(Policy = "FiveYearsEmployee")]
        public IActionResult Special()
        {
            return View();
        }

        [Authorize(Policy = "AdminClaimPolicy")]
        public IActionResult AdminClaimRequired()
        {
            return View();
        }

    }
}
