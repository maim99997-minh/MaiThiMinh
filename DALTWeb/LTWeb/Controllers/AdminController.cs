using LTWeb.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace LTWeb.Controllers
{
    // [Authorize] đảm bảo chỉ người đã đăng nhập mới vào được các trang này
    [Authorize]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AdminController(ApplicationDbContext db)
        {
            _db = db;
        }

        // 1. DÀNH CHO ADMIN: Xem toàn bộ đơn hàng của tất cả khách hàng
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> OrderList()
        {
            // Sử dụng ToListAsync để tăng hiệu năng xử lý bất đồng bộ
            var allOrders = await _db.Orders
                                     .OrderByDescending(o => o.OrderDate)
                                     .ToListAsync();

            // Nếu danh sách null, khởi tạo danh sách rỗng để tránh lỗi View
            if (allOrders == null) allOrders = new System.Collections.Generic.List<Models.Order>();

            ViewBag.Title = "Quản lý toàn bộ đơn hàng";
            return View(allOrders);
        }

        // 2. DÀNH CHO KHÁCH HÀNG: Chỉ xem lịch sử đơn hàng của chính mình
        public async Task<IActionResult> MyOrders()
        {
            // Lấy Email của người dùng hiện tại từ Identity
            var userEmail = User.Identity.Name;

            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login", "Account");
            }

            // Lọc đơn hàng dựa trên Email đã lưu trong Database
            var myOrders = await _db.Orders
                                    .Where(o => o.Email == userEmail)
                                    .OrderByDescending(o => o.OrderDate)
                                    .ToListAsync();

            ViewBag.Title = "Đơn hàng của tôi";

            // Tận dụng lại View "OrderList" để tiết kiệm thời gian code giao diện
            return View("OrderList", myOrders);
        }
        public async Task<IActionResult> Details(int id)
        {
            // 1. Lấy thông tin đơn hàng tổng quát
            var order = await _db.Orders.FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            // 2. Lấy danh sách các món ăn trong đơn hàng này (Kết hợp 2 bảng OrderDetails và SeafoodItems để lấy được Tên món và Ảnh)
            var orderDetails = await (from od in _db.OrderDetails
                                      join p in _db.SeafoodItems on od.ProductId equals p.Id
                                      where od.OrderId == id
                                      select new
                                      {
                                          ProductName = p.Name,
                                          Image = p.Image,
                                          Quantity = od.Quantity,
                                          Price = od.Price
                                      }).ToListAsync();

            // Truyền danh sách món ăn sang View thông qua ViewBag
            ViewBag.OrderDetails = orderDetails;

            return View(order);
        }
    }
}