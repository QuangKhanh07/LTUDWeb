using Microsoft.AspNetCore.Mvc;

namespace SV21T1020037.Shop.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult AccessDenined()
        {
            return View();
        }
        public IActionResult ChangePassword()
        {
            return View();
        }
        public IActionResult Information()
        {
            return View();
        }
        public IActionResult EditInformation()
        {
            return View();
        }
    }
}
