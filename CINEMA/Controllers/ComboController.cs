using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CINEMA.Models;

namespace CINEMA.Controllers
{
    public class ComboController : Controller
    {
        private readonly CinemaContext _context;

        public ComboController(CinemaContext context)
        {
            _context = context;
        }

        // 🧱 Hàm kiểm tra trạng thái đăng nhập admin
        private bool IsAdminLoggedIn()
        {
            return HttpContext.Session.GetString("Role") == "Admin";
        }

        // ---------------- INDEX ----------------
        public async Task<IActionResult> Index()
        {
            if (!IsAdminLoggedIn())
                return RedirectToAction("Login", "Admin");

            var combos = await _context.Combos.ToListAsync();
            return View(combos);
        }

        // ---------------- DETAILS ----------------
        public async Task<IActionResult> Details(int? id)
        {
            if (!IsAdminLoggedIn())
                return RedirectToAction("Login", "Admin");

            if (id == null)
                return NotFound();

            var combo = await _context.Combos.FirstOrDefaultAsync(m => m.ComboId == id);
            if (combo == null)
                return NotFound();

            return View(combo);
        }

        // ---------------- CREATE ----------------
        [HttpGet]
        public IActionResult Create()
        {
            if (!IsAdminLoggedIn())
                return RedirectToAction("Login", "Admin");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ComboId,Name,Description,Price,ImageUrl,IsActive")] Combo combo)
        {
            if (!IsAdminLoggedIn())
                return RedirectToAction("Login", "Admin");

            if (ModelState.IsValid)
            {
                _context.Add(combo);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "✅ Thêm combo thành công!";
                return RedirectToAction(nameof(Index));
            }

            return View(combo);
        }

        // ---------------- EDIT ----------------
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsAdminLoggedIn())
                return RedirectToAction("Login", "Admin");

            if (id == null)
                return NotFound();

            var combo = await _context.Combos.FindAsync(id);
            if (combo == null)
                return NotFound();

            return View(combo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ComboId,Name,Description,Price,ImageUrl,IsActive")] Combo combo)
        {
            if (!IsAdminLoggedIn())
                return RedirectToAction("Login", "Admin");

            if (id != combo.ComboId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(combo);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "✅ Cập nhật combo thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ComboExists(combo.ComboId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            return View(combo);
        }

        // ---------------- DELETE ----------------
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (!IsAdminLoggedIn())
                return RedirectToAction("Login", "Admin");

            if (id == null)
                return NotFound();

            var combo = await _context.Combos.FirstOrDefaultAsync(m => m.ComboId == id);
            if (combo == null)
                return NotFound();

            return View(combo);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsAdminLoggedIn())
                return RedirectToAction("Login", "Admin");

            var combo = await _context.Combos.FindAsync(id);
            if (combo != null)
            {
                _context.Combos.Remove(combo);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "🗑 Xóa combo thành công!";
            }

            return RedirectToAction(nameof(Index));
        }

        // ---------------- CHECK EXIST ----------------
        private bool ComboExists(int id)
        {
            return _context.Combos.Any(e => e.ComboId == id);
        }
    }
}
