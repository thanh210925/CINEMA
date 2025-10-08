using Microsoft.AspNetCore.Mvc;
using CINEMA.Models;
using Microsoft.EntityFrameworkCore;

namespace CINEMA.Controllers
{
    public class AuditoriumController : Controller
    {
        private readonly CinemaContext _context;

        public AuditoriumController(CinemaContext context)
        {
            _context = context;
        }

        // GET: /Auditorium
        public async Task<IActionResult> Index()
        {
            var list = await _context.Auditoriums.Include(a => a.Theater).ToListAsync();
            return View(list);
        }

        // GET: /Auditorium/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var auditorium = await _context.Auditoriums
                .Include(a => a.Theater)
                .FirstOrDefaultAsync(a => a.AuditoriumId == id);

            if (auditorium == null) return NotFound();

            return View(auditorium);
        }

        // GET: /Auditorium/Create
        public IActionResult Create()
        {
            ViewData["Theaters"] = _context.Theaters.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Auditorium auditorium)
        {
            if (ModelState.IsValid)
            {
                _context.Add(auditorium);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Theaters"] = _context.Theaters.ToList();
            return View(auditorium);
        }

        // GET: /Auditorium/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var auditorium = await _context.Auditoriums.FindAsync(id);
            if (auditorium == null) return NotFound();

            ViewData["Theaters"] = _context.Theaters.ToList();
            return View(auditorium);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Auditorium auditorium)
        {
            if (id != auditorium.AuditoriumId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(auditorium);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Auditoriums.Any(e => e.AuditoriumId == id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["Theaters"] = _context.Theaters.ToList();
            return View(auditorium);
        }

        // GET: /Auditorium/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var auditorium = await _context.Auditoriums
                .Include(a => a.Theater)
                .FirstOrDefaultAsync(a => a.AuditoriumId == id);

            if (auditorium == null) return NotFound();

            return View(auditorium);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var auditorium = await _context.Auditoriums.FindAsync(id);
            if (auditorium != null)
            {
                _context.Auditoriums.Remove(auditorium);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
