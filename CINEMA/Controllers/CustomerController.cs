using CINEMA.Models;
using Microsoft.AspNetCore.Mvc;

namespace CINEMA.Controllers
{
    public class CustomerController : Controller
    {
        private readonly CinemaContext _context;

        public CustomerController(CinemaContext context)
        {
            _context = context;
        }

        // GET: Register
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(Customer customer)
        {
            if (ModelState.IsValid)
            {
                var exist = _context.Customers.FirstOrDefault(c => c.Email == customer.Email);
                if (exist != null)
                {
                    ViewBag.Error = "Email đã tồn tại!";
                    return View(customer);
                }

                customer.CreatedAt = DateTime.Now;

                _context.Customers.Add(customer);
                _context.SaveChanges();

                // Sau khi đăng ký thành công → chuyển sang Login
                return RedirectToAction("Login", "Customer");
            }

            ViewBag.Error = "Dữ liệu không hợp lệ!";
            return View(customer);
        }

        // GET: Login
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var customer = _context.Customers
                .FirstOrDefault(c => c.Email == email && c.PasswordHash == password);

            if (customer == null)
            {
                ViewBag.Error = "Sai tài khoản hoặc mật khẩu!";
                return View();
            }

            // Cập nhật LastLogin
            customer.LastLogin = DateTime.Now;
            _context.SaveChanges();

            // Lưu Session
            HttpContext.Session.SetString("CustomerName", customer.FullName);

            return RedirectToAction("Index", "Home");
        }

        // GET: Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Customer");
        }
    }
}
