using LTWeb.Data;
using LTWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace LTWeb.Controllers
{
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hiển thị trang đặt bàn
        public IActionResult Index() => View();

        // Xử lý gửi form
        [HttpPost]
        public async Task<IActionResult> Create(Booking booking)
        {
            if (ModelState.IsValid)
            {
                _context.Add(booking);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Đặt bàn thành công! Chúng tôi sẽ liên hệ sớm.";
                return RedirectToAction("Index");
            }
            return View("Index", booking);
        }
    }
}
