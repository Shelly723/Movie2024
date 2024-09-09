using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movie2024.Models;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Movie2024.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly movieContext _movieContext; // 先在全域宣告資料庫物件

        public HomeController(ILogger<HomeController> logger, movieContext movieContext) // 依賴注入使用我們剛設定好的資料庫物件的寫法
        {
            _logger = logger;
            _movieContext = movieContext;
        }

        public IActionResult Index()
        {
            var model = _movieContext.Users.FirstOrDefault();
            TempData.Keep("title2");
            return View(model);
        }

        public IActionResult Privacy()
        {
            var model = _movieContext.Movies.ToList();
            return View(model);
        }

        public IActionResult Order()
        {
            return View();
        }

        public IActionResult OrderSeat()
        {
            var model = _movieContext.Seats.ToList();
            return View(model);
        }

        [HttpPost]
        public IActionResult UpdateAvailability([FromBody] SeatUpdateModel model)
        {
            // 查找座位
            var seat = _movieContext.Seats.SingleOrDefault(s => s.SeatNumber == model.SeatNumber);
            if (seat != null)
            {
                seat.IsAvailable = "N";
                _movieContext.SaveChanges();
                return Ok();
            }
            return NotFound();
        }

        public class SeatUpdateModel
        {
            public string SeatNumber { get; set; } = null!;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
