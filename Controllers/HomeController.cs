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
        private readonly movieContext _movieContext; // ���b����ŧi��Ʈw����

        public HomeController(ILogger<HomeController> logger, movieContext movieContext) // �̿�`�J�ϥΧڭ̭�]�w�n����Ʈw���󪺼g�k
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
            // �p�G�䤣��Τ� ID �εL�k�ѪR�A�i�H�ߥX���`�Ϊ�^�@���q�{��
            //throw new ApplicationException("�L�k�����e�Τ� ID");
            return 0;
        }

        public IActionResult Index()    //����
        {
            var model = _movieContext.Movies.ToList();

            var userId = GetCurrentUserID(); // ���o�ثe�Τ� ID
            var user = userId > 0 ? _movieContext.Users.FirstOrDefault(u => u.UserID == userId) : null;

            ViewBag.UserName = user != null ? user.UserName : "�Q��"; 

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

        public IActionResult Privacy()  //�q�v����
        {
            var model = _movieContext.Movies.ToList();
            return View(model);
        }

        [Authorize]
        public IActionResult OrderSeat(int id)
        {

            var movie = _movieContext.Movies.Find(id);

            // �ھ� MovieID �d��Ҧ�����
            var showtimes = _movieContext.Showtimes
                .Where(s => s.MovieID == id)
                .ToList();

            var userId = GetCurrentUserID(); // ���o�ثe�Τ� ID

            var user = _movieContext.Users.FirstOrDefault(u => u.UserID == userId);
            if (user != null)
            {
                ViewBag.UserName = user.UserName; // �N UserName �ǻ���e��
            }
            else
            {
                ViewBag.UserName = "�������ϥΪ�"; // �p�G�L�k���Τ�A����q�{��
            }

            if (movie == null || !showtimes.Any())
            {
                return NotFound();
            }

            // ���Ҧ������� TheaterID
            var theaterIds = showtimes.Select(s => s.TheaterID).Distinct().ToList();
            // �ھ� TheaterID �d��Ҧ��������v�U���
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
            // �ѪR��w���y�� ID
            var seatIDs = JsonConvert.DeserializeObject<List<int>>(SelectedSeats);

            // �Ыطs���q��
            var order = new Orders
            {
                UserID = GetCurrentUserID(), // ���]�z���@�Ӥ�k�������e�Τ� ID
                ShowtimeID = ShowtimeID,
                TotalAmount = TotalAmount,
                CreatedAt = DateTime.Now
            };

            _movieContext.Orders.Add(order);
            await _movieContext.SaveChangesAsync();

            // ���C�ӿ�w���y��Ы� OrderSeat �O��
            foreach (var seatID in seatIDs)
            {
                var orderSeat = new OrderSeats
                {
                    OrderID = order.OrderID,
                    SeatID = seatID
                };
                _movieContext.OrderSeats.Add(orderSeat);

                // ��s ShowtimeSeats �������y�쪺�i�Ϊ��A
                var showtimeSeat = await _movieContext.ShowtimeSeats
                    .FirstOrDefaultAsync(ss => ss.ShowtimeID == ShowtimeID && ss.SeatID == seatID);
                if (showtimeSeat == null)
                {
                    // �p�G���s�b�A�Ыطs�O��
                    showtimeSeat = new ShowtimeSeats
                    {
                        ShowtimeID = ShowtimeID,
                        SeatID = seatID,
                        IsAvailable = "Y"  // ��l���A�]���i��
                    };
                    _movieContext.ShowtimeSeats.Add(showtimeSeat);
                }
                if (showtimeSeat != null)
                {
                    showtimeSeat.IsAvailable = "N";  // �]�w�Ӯy�쬰���i��
                }
            }

            await _movieContext.SaveChangesAsync();


            return RedirectToAction("OrderSuccess", new { orderId = order.OrderID });
        }
        [HttpGet]
        public IActionResult GetBookedSeats(int showtimeID)
        {
            // �d��ӳ������w�Q�w�q���y��
            var bookedSeats = _movieContext.ShowtimeSeats
                .Where(ss => ss.ShowtimeID == showtimeID && ss.IsAvailable == "N")
                .Select(ss => ss.Seats.SeatNumber) // ��ܮy��s��
                .ToList();

            return Json(new { bookedSeats });
        }

        public IActionResult OrderSuccess(int orderId)  //�̲׭q�檺�ԲӫH��
        {     
            var order = _movieContext.Orders
                .Include(o => o.OrderSeats) // �]�t�P�y�쪺���p
                .ThenInclude(os => os.Seats)  // �]�t�y��ԲӸ��
                .Include(o => o.Showtimes)    // �]�t�����ԲӸ��
                .ThenInclude(s => s.Movies)  // �o�̽T�O�����s�a�q�v�W�ٳQ�d�X��
                .FirstOrDefault(o => o.OrderID == orderId);

            if (order == null)
            {
                return NotFound();
            }

            // �N�q���ƶǻ������
            return View(order);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
