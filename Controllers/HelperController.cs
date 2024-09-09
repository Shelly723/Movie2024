using Microsoft.AspNetCore.Mvc;
using Movie2024.Models;
using System.Linq;

namespace Movie2024.Controllers
{
    public class HelperController : Controller
    {
        private readonly movieContext _context;

        public HelperController(movieContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string id, string password)
        {
            // 查詢使用者帳號和密碼
            var user = _context.Users.FirstOrDefault(u => u.UserName == id && u.Password == password);

            if (user != null)
            {
                // 如果驗證成功，跳轉到 Home 控制器的 Index 動作
                return RedirectToAction("Orderseat", "Home");
            }
         
           
            // 如果驗證失敗，回傳錯誤訊息並留在登入頁面
            ViewBag.ErrorMessage = "驗證失敗";
            return View("Index");
        }
        public IActionResult Login2()
        {
            return View();
        }
    }
}
