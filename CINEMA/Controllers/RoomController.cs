using CINEMA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CINEMA.Controllers
{
    public class RoomController : Controller
    {
        private readonly CinemaContext _context;

        public RoomController(CinemaContext context)
        {
            _context = context;
        }

        // 🔐 Kiểm tra đăng nhập admin
        private bool IsAdminLoggedIn()
        {
            return HttpContext.Session.GetString("Role") == "Admin";
        }

        // 📋 Danh sách phòng chiếu
        public async Task<IActionResult> Index()
        {
            if (!IsAdminLoggedIn())
                return RedirectToAction("Login", "Admin");

            var rooms = await _context.Auditoriums
                .Include(r => r.Theater)
                .ToListAsync();
            return View(rooms);
        }

        // 🔍 Chi tiết
        public async Task<IActionResult> Details(int? id)
        {
            if (!IsAdminLoggedIn())
                return RedirectToAction("Login", "Admin");

            if (id == null)
                return NotFound();

            var room = await _context.Auditoriums
                .Include(r => r.Theater)
                .FirstOrDefaultAsync(m => m.AuditoriumId == id);

            if (room == null)
                return NotFound();

            return View(room);
        }

        // ➕ Tạo mới
        public IActionResult Create()
        {
            if (!IsAdminLoggedIn())
                return RedirectToAction("Login", "Admin");

            ViewBag.Theaters = _context.Theaters?.ToList() ?? new List<Theater>();
            var model = new Auditorium { IsActive = true }; // mặc định đang hoạt động
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Auditorium room)
        {
            if (ModelState.IsValid)
            {
                _context.Add(room);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Theaters = _context.Theaters?.ToList() ?? new List<Theater>();
            return View(room);
        }


        // ✏️ Chỉnh sửa
        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsAdminLoggedIn())
                return RedirectToAction("Login", "Admin");

            if (id == null)
                return NotFound();

            var room = await _context.Auditoriums.FindAsync(id);
            if (room == null)
                return NotFound();

            ViewData["Theaters"] = _context.Theaters.ToList();
            return View(room);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Auditorium room)
        {
            if (id != room.AuditoriumId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(room);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Auditoriums.Any(e => e.AuditoriumId == room.AuditoriumId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["Theaters"] = _context.Theaters.ToList();
            return View(room);
        }

        // 🗑️ Xóa
        // GET: Room/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!IsAdminLoggedIn())
                return RedirectToAction("Login", "Admin");

            if (id == null)
                return NotFound();

            var room = await _context.Auditoriums
                .Include(r => r.Theater)
                .FirstOrDefaultAsync(m => m.AuditoriumId == id);

            if (room == null)
                return NotFound();

            return View(room);
        }

        // POST: Room/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var room = await _context.Auditoriums.FindAsync(id);
            if (room != null)
            {
                _context.Auditoriums.Remove(room);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

    }
}
