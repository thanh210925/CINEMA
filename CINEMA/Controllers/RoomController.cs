using Microsoft.AspNetCore.Mvc;
using CINEMA.Models;
using System.Linq;

namespace CINEMA.Controllers
{
    public class RoomController : Controller
    {
        private readonly CinemaContext _context;
        public RoomController(CinemaContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách phòng
        public IActionResult Index()
        {
            var rooms = _context.Rooms.ToList();
            return View(rooms);
        }

        // Thêm mới
        public IActionResult Create() => View();

        [HttpPost]
        public IActionResult Create(Room room)
        {
            if (ModelState.IsValid)
            {
                _context.Rooms.Add(room);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(room);
        }

        // Sửa
        public IActionResult Edit(int id)
        {
            var room = _context.Rooms.Find(id);
            if (room == null) return NotFound();
            return View(room);
        }

        [HttpPost]
        public IActionResult Edit(Room room)
        {
            if (ModelState.IsValid)
            {
                _context.Rooms.Update(room);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(room);
        }

        // Xoá
        public IActionResult Delete(int id)
        {
            var room = _context.Rooms.Find(id);
            if (room == null) return NotFound();
            _context.Rooms.Remove(room);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
       

    }

}
