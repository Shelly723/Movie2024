using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Movie2024.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

namespace Movie2024.Controllers
{
    public class HelperController : Controller
    {
        private readonly movieContext _movieContext;

        public HelperController(movieContext movieContext)
        {
            _movieContext = movieContext;
        }

        public IActionResult Index(string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string id, string password, string returnUrl)
        {
            var user = _movieContext.Users.FirstOrDefault(u => u.UserName == id && u.Password == password);

            if (user != null)
            {
                // 建立使用者的聲明 (Claims)
                var claims = new List<Claim>
                {
                     new Claim(ClaimTypes.Name, user.UserName),
                     new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString())
                };

                // 建立身份認證 (ClaimsIdentity)
                var claimsIdentity = new ClaimsIdentity(claims, "CookieAuthentication");

                // 建立認證原則 (ClaimsPrincipal)
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                // 執行登入，建立認證的 Cookie
                await HttpContext.SignInAsync("CookieAuthentication", claimsPrincipal);

                // 登入成功後的重導向
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            // 登入失敗的處理
            ViewBag.ErrorMessage = "驗證失敗";
       
            return View("Index");
        }
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
        public IActionResult CreateUser()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreateUser(string UserName, string Password, string Email, string PhoneNumber)
        {
            var hasError=false;
            // 檢查 UserName 是否已存在
            var existingUser = _movieContext.Users.FirstOrDefault(u => u.UserName == UserName);
            if (existingUser != null)
            {
                ModelState.AddModelError("UserName", "此使用者名稱已被使用。");
                hasError = true;
            }

            // 檢查 Email 是否已存在
            var existingEmail = _movieContext.Users.FirstOrDefault(u => u.Email == Email);
            if (existingEmail != null)
            {
                ModelState.AddModelError("Email", "此Email已被使用。");
                hasError = true;
            }
            // 檢查 PhoneNumber 是否已存在
            var existingPhoneNumber = _movieContext.Users.FirstOrDefault(u => u.PhoneNumber == PhoneNumber);
            if (existingPhoneNumber != null)
            {
                ModelState.AddModelError("PhoneNumber", "此手機號碼已被使用。");
                hasError = true;
            }
            // 如果有錯誤，返回註冊視圖並保留輸入的資料
            if (hasError || !ModelState.IsValid)
            {
                return View("Register", new Users
                {
                    UserName = UserName,
                    Password= "",
                    Email = Email,
                    PhoneNumber = PhoneNumber
                });
            }
            // 建立新的使用者物件
            var newUser = new Users
            {
                UserName = UserName,
                Password = Password,
                Email = Email,
                PhoneNumber = PhoneNumber,
                CreatedAt = DateTime.Now,
                isStaff = "N" // 預設為一般會員
            };

            // 新增使用者到資料庫
            _movieContext.Users.Add(newUser);
            _movieContext.SaveChanges();

            // 註冊成功後跳轉到登入頁面
            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuthentication");
            return RedirectToAction("Index", "Home");
        }

    }
}
