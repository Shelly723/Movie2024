using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Movie2024.Models;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
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

        protected int GetCurrentUserID()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            // 如果找不到用戶 ID 或無法解析，可以拋出異常或返回一個默認值
            //throw new ApplicationException("無法獲取當前用戶 ID");
            return 0;
        }

        public IActionResult Index()    //首頁
        {
            var model = _movieContext.Movies.ToList();

            var userId = GetCurrentUserID(); // 取得目前用戶 ID
            var user = userId > 0 ? _movieContext.Users.FirstOrDefault(u => u.UserID == userId) : null;

            ViewBag.UserName = user != null ? user.UserName : "貴賓"; 

            return View(model);
        }
        public IActionResult Introduce()
        {
            return View();
        }
        public IActionResult Index1()
        {
            var model = _movieContext.Seats.ToList();
            return View(model);
        }

        public IActionResult Privacy()  //電影介紹
        {
            var model = _movieContext.Movies.ToList();
            return View(model);
        }

        [Authorize]
        public IActionResult OrderSeat(int id)
        {

            var movie = _movieContext.Movies.Find(id);

            // 根據 MovieID 查找所有場次
            var showtimes = _movieContext.Showtimes
                .Where(s => s.MovieID == id)
                .ToList();

            var userId = GetCurrentUserID(); // 取得目前用戶 ID

            var user = _movieContext.Users.FirstOrDefault(u => u.UserID == userId);
            if (user != null)
            {
                ViewBag.UserName = user.UserName; // 將 UserName 傳遞到前端
            }
            else
            {
                ViewBag.UserName = "未知的使用者"; // 如果無法找到用戶，顯示默認值
            }

            if (movie == null || !showtimes.Any())
            {
                return NotFound();
            }

            // 找到所有對應的 TheaterID
            var theaterIds = showtimes.Select(s => s.TheaterID).Distinct().ToList();
            // 根據 TheaterID 查找所有對應的影廳資料
            var theaters = _movieContext.Theaters
                .Where(t => theaterIds.Contains(t.TheaterID))
                .ToList();
            var theater = theaters.FirstOrDefault();

            var seats = _movieContext.Seats
                .Where(s => theaterIds.Contains(s.TheaterID))
                .ToList();

            var viewModel = new MoviesAndSeats
            {
                Movies = movie,
                Showtimes = showtimes,
                Seats = seats,
                Theaters = theater
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> OrderCheck(int ShowtimeID, string SelectedSeats, decimal TotalAmount)
        {
            // 解析選定的座位 ID
            var seatIDs = JsonConvert.DeserializeObject<List<int>>(SelectedSeats);

            // 創建新的訂單
            var order = new Orders
            {
                UserID = GetCurrentUserID(), // 假設您有一個方法來獲取當前用戶 ID
                ShowtimeID = ShowtimeID,
                TotalAmount = TotalAmount,
                CreatedAt = DateTime.Now
            };

            _movieContext.Orders.Add(order);
            await _movieContext.SaveChangesAsync();

            // 為每個選定的座位創建 OrderSeat 記錄
            foreach (var seatID in seatIDs)
            {
                var orderSeat = new OrderSeats
                {
                    OrderID = order.OrderID,
                    SeatID = seatID
                };
                _movieContext.OrderSeats.Add(orderSeat);

                // 更新 ShowtimeSeats 表中對應座位的可用狀態
                var showtimeSeat = await _movieContext.ShowtimeSeats
                    .FirstOrDefaultAsync(ss => ss.ShowtimeID == ShowtimeID && ss.SeatID == seatID);
                if (showtimeSeat == null)
                {
                    // 如果不存在，創建新記錄
                    showtimeSeat = new ShowtimeSeats
                    {
                        ShowtimeID = ShowtimeID,
                        SeatID = seatID,
                        IsAvailable = "Y"  // 初始狀態設為可用
                    };
                    _movieContext.ShowtimeSeats.Add(showtimeSeat);
                }
                if (showtimeSeat != null)
                {
                    showtimeSeat.IsAvailable = "N";  // 設定該座位為不可用
                }
            }

            await _movieContext.SaveChangesAsync();


            return RedirectToAction("OrderSuccess", new { orderId = order.OrderID });
        }
        [HttpGet]
        public IActionResult GetBookedSeats(int showtimeID)
        {
            // 查找該場次中已被預訂的座位
            var bookedSeats = _movieContext.ShowtimeSeats
                .Where(ss => ss.ShowtimeID == showtimeID && ss.IsAvailable == "N")
                .Select(ss => ss.Seats.SeatNumber) // 選擇座位編號
                .ToList();

            return Json(new { bookedSeats });
        }

        public IActionResult OrderSuccess(int orderId)  //最終訂單的詳細信息
        {     
            var order = _movieContext.Orders
                .Include(o => o.OrderSeats) // 包含與座位的關聯
                .ThenInclude(os => os.Seats)  // 包含座位詳細資料
                .Include(o => o.Showtimes)    // 包含場次詳細資料
                .ThenInclude(s => s.Movies)  // 這裡確保場次連帶電影名稱被查出來
                .FirstOrDefault(o => o.OrderID == orderId);

            if (order == null)
            {
                return NotFound();
            }

            // 將訂單資料傳遞到視圖
            return View(order);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
