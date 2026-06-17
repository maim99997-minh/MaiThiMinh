using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LTWeb.Data; // Thay bằng namespace Data của bạn
using LTWeb.Models;
using System.Security.Claims;

namespace LTWeb.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Trang danh sách đơn hàng của tôi
        public async Task<IActionResult> Index()
        {
            // Lấy email của người dùng đang đăng nhập để lọc đơn hàng
            var userEmail = User.Identity.Name;

            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login", "Account");
            }

            var orders = await _context.Orders
                .Where(o => o.Email == userEmail)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }
    }
}