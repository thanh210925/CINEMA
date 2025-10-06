using Microsoft.AspNetCore.Mvc;
using CINEMA.Models;
using System.Linq;

namespace CINEMA.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly CinemaContext _context;
        public StatisticsController(CinemaContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("Login", "Admin");

            ViewBag.OrderCount = _context.Orders.Count();
            ViewBag.TicketCount = _context.Tickets.Count();
            ViewBag.ComboCount = _context.Combos.Count();
            ViewBag.TotalRevenue = _context.Orders.Sum(o => o.TotalAmount);

            // Dữ liệu biểu đồ demo
            ViewBag.Labels = new[] { "T2", "T3", "T4", "T5", "T6", "T7", "CN" };
            ViewBag.Data = new[] { 800000, 1200000, 900000, 1500000, 1100000, 1800000, 1300000 };

            return View();
        }
    }
}
