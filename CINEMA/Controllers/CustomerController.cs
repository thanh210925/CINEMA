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
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: Register
        [HttpPost]
        public IActionResult Register(Customer customer)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra email trùng
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
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Login
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

            // Cập nhật lần đăng nhập cuối
            customer.LastLogin = DateTime.Now;
            _context.SaveChanges();

            // Lưu session
            HttpContext.Session.SetString("CustomerName", customer.FullName);
            HttpContext.Session.SetString("CustomerEmail", customer.Email);

            return RedirectToAction("Index", "Home");
        }

        // GET: Forgot Password
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // POST: Forgot Password
        [HttpPost]
        public IActionResult ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ViewBag.Error = "Vui lòng nhập email.";
                return View();
            }

            // Kiểm tra email có trong DB
            var customer = _context.Customers.FirstOrDefault(c => c.Email == email);
            if (customer == null)
            {
                ViewBag.Message = $"Nếu email {email} tồn tại, chúng tôi đã gửi hướng dẫn đặt lại mật khẩu.";
                return View();
            }

            // TODO: Thực hiện gửi email reset mật khẩu (SMTP hoặc MailKit)
            ViewBag.Message = $"Hướng dẫn đặt lại mật khẩu đã được gửi đến email {email}.";
            return View();
        }

        // GET: Logout
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Customer");
        }
    }
}
