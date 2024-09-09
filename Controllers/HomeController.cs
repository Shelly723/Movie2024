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
        private readonly movieContext _movieContext; // ���b����ŧi��Ʈw����

        public HomeController(ILogger<HomeController> logger, movieContext movieContext) // �̿�`�J�ϥΧڭ̭�]�w�n����Ʈw���󪺼g�k
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
            // �d��y��
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
