using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020037.BusinessLayers;

namespace SV21T1020037.Web.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string userName, string password)
        {
            ViewBag.UserName = userName;

            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("Error", "Vui lòng nhập Tài khoản và Mật Khẩu");
                return View();
            }

            var userAccount = UserAccountService.Authorize(UserTypes.Employee, userName, password);
            if (userAccount == null)
            {
                ModelState.AddModelError("Error", "Đăng nhập thất bại");
                return View();
            }

            //Login success: login status
            var userData = new WebUserData()
            {
                UserId = userAccount.UserId,
                UserName = userAccount.UserName,
                DisplayName = userAccount.DisplayName,
                Photo = userAccount.Photo,
                Roles = userAccount.RoleNames.Split(',').ToList()

            };
            await HttpContext.SignInAsync(userData.CreatePrincipal());

            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenined()
        {
            return View();
        }
        public IActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ChangePassword(string UserName, string oldPassword, string newPassword, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(oldPassword) || string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                ModelState.AddModelError("Error", "Vui lòng nhập đầy đủ thông tin.");
                return View();
            }
            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError("Error", "Mật khẩu mới không khớp");
                return View();
            }
            if (ModelState.IsValid == false)// !ModelState.IsValid
            {
                return View();
            }
            try
            {
                var userAccount = UserAccountService.Authorize(UserTypes.Employee, UserName, oldPassword);
                if (userAccount == null)
                {
                    ModelState.AddModelError("Error", "Mật khẩu cũ không chính xác");
                    return View();
                }
                else
                {
                    bool result = UserAccountService.ChangedPassword(UserName, newPassword);
                    if (result)
                    {
                        TempData["ChangedPassword"] = "Đổi mật khẩu thành công!";
                        return View();
                    }
                    else
                    {
                        ModelState.AddModelError("Error", "Đổi mật khẩu thất bại.");
                        return View();
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "Hệ thống tạm thời gián đoạn");
                return View();
            }
        }

    }
}
