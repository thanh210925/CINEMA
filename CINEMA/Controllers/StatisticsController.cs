using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CINEMA.Models;
using System;
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

        public IActionResult Index(DateTime? startDate, DateTime? endDate, string type = "revenue")
        {
            // ✅ Kiểm tra đăng nhập admin
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("Login", "Admin");

            // Mặc định thống kê tháng hiện tại
            if (startDate == null)
                startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            if (endDate == null)
                endDate = startDate.Value.AddMonths(1).AddDays(-1);

            ViewBag.StartDate = startDate.Value.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate.Value.ToString("yyyy-MM-dd");
            ViewBag.SelectedType = type;

            // ✅ Tổng quan
            ViewBag.OrderCount = _context.Orders.Count();
            ViewBag.TicketCount = _context.Tickets.Count();
            ViewBag.ComboCount = _context.Combos.Count();
            ViewBag.TotalRevenue = _context.Orders.Sum(o => o.TotalAmount) ?? 0;

            // ✅ Chuẩn bị dữ liệu cho biểu đồ chính
            string chartTitle;
            var labels = new List<string>();
            var data = new List<double>();

            if (type == "revenue")
            {
                chartTitle = "Doanh thu theo ngày";
                var orders = _context.Orders
                    .Where(o => o.CreatedAt >= startDate && o.CreatedAt <= endDate)
                    .GroupBy(o => o.CreatedAt!.Value.Date)
                    .Select(g => new { Date = g.Key, Total = g.Sum(o => o.TotalAmount ?? 0) })
                    .OrderBy(x => x.Date)
                    .ToList();

                foreach (var item in orders)
                {
                    labels.Add(item.Date.ToString("dd/MM"));
                    data.Add((double)item.Total);
                }
            }
            else if (type == "ticket")
            {
                chartTitle = "Số vé bán theo ngày";
                var tickets = _context.Tickets
                    .Where(t => t.BookedAt >= startDate && t.BookedAt <= endDate)
                    .GroupBy(t => t.BookedAt!.Value.Date)
                    .Select(g => new { Date = g.Key, Count = g.Count() })
                    .OrderBy(x => x.Date)
                    .ToList();

                foreach (var item in tickets)
                {
                    labels.Add(item.Date.ToString("dd/MM"));
                    data.Add(item.Count);
                }
            }
            else
            {
                chartTitle = "Số combo bán theo ngày";
                var combos = _context.OrderCombos
                    .Include(oc => oc.Order)
                    .Where(oc => oc.Order!.CreatedAt >= startDate && oc.Order.CreatedAt <= endDate)
                    .GroupBy(oc => oc.Order!.CreatedAt!.Value.Date)
                    .Select(g => new { Date = g.Key, Quantity = g.Sum(x => x.Quantity ?? 0) })
                    .OrderBy(x => x.Date)
                    .ToList();

                foreach (var item in combos)
                {
                    labels.Add(item.Date.ToString("dd/MM"));
                    data.Add(item.Quantity);
                }
            }

            ViewBag.ChartTitle = chartTitle;
            ViewBag.Labels = labels;
            ViewBag.Data = data;

            // ✅ Top 5 phim bán chạy
            var topMovies = _context.Tickets
                .Include(t => t.Showtime)
                .ThenInclude(s => s.Movie)
                .Where(t => t.BookedAt >= startDate && t.BookedAt <= endDate && t.Showtime!.Movie != null)
                .GroupBy(t => t.Showtime!.Movie!.Title)
                .Select(g => new
                {
                    MovieName = g.Key,
                    TicketCount = g.Count(),
                    Revenue = g.Sum(t => t.Price ?? 0)
                })
                .OrderByDescending(x => x.Revenue)
                .Take(5)
                .ToList();

            ViewBag.TopMovies = topMovies;

            return View();
        }
    }
}
