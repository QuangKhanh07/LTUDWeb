using Microsoft.AspNetCore.Mvc;
using SV21T1020037.Web.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
namespace SV21T1020037.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.ADMINISTRATOR}, {WebUserRoles.EMPLOYEE}")]

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
    }
}
