using System.Diagnostics;
using CINEMA.Models;
using Microsoft.AspNetCore.Mvc;

namespace CINEMA.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CinemaContext _context;

        public HomeController(ILogger<HomeController> logger, CinemaContext context)
        {
            _logger = logger;
            _context = context;
        }

        // Trang ch? - hi?n th? phim ?ang chi?u
        public IActionResult Index()
        {
            // L?y các phim ?ang active t? DB
            var movies = _context.Movies
                .Where(m => m.IsActive == true)
                .OrderByDescending(m => m.ReleaseDate)
                .ToList();

            return View(movies); // truy?n danh sách phim ra view
        }

        // Trang ??t vé (sau khi b?m "??t vé ngay")
        public IActionResult BookTicket(int id)
        {
            var movie = _context.Movies.FirstOrDefault(m => m.MovieId == id);
            if (movie == null) return NotFound();

            return View(movie);
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
