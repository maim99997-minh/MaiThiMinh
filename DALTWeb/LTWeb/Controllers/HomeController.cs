using LTWeb.Data;
using LTWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;  
using System.Threading.Tasks;

namespace LTWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult About() => View();


        // --- HỆ THỐNG MENU ---
        public async Task<IActionResult> Menu() => View(await _context.SeafoodItems.Where(x => x.Category == "CÁC LOẠI HẢI SẢN KHÁC").ToListAsync());

        public async Task<IActionResult> Menu2() => View(await _context.SeafoodItems.Where(x => x.Category == "CÁ").ToListAsync());

        public async Task<IActionResult> Menu3() => View(await _context.SeafoodItems.Where(x => x.Category == "ĐẶC BIỆT").ToListAsync());

        public async Task<IActionResult> Menu4() => View(await _context.SeafoodItems.Where(x => x.Category == "CUA - GHẸ").ToListAsync());

        public async Task<IActionResult> Menu5() => View(await _context.SeafoodItems.Where(x => x.Category == "BÀO NGƯ - HÀU").ToListAsync());

        // --- XỬ LÝ ĐẶT BÀN ---
        [HttpGet]
        public async Task<IActionResult> DatBan()
        {
            // Nếu là Admin, nạp danh sách vào ViewBag để hiển thị bảng quản lý bên dưới form
            if (User.IsInRole("Admin"))
            {
                ViewBag.AdminBookingList = await _context.Bookings
                    .OrderByDescending(x => x.CreatedAt)
                    .ToListAsync();
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        // BỔ SUNG "NumberOfPeople" VÀO DANH SÁCH BIND DƯỚI ĐÂY:
        public async Task<IActionResult> DatBan([Bind("CustomerName,Email,Phone,Branch,BookingDate,NumberOfPeople,Note")] Booking booking)
        {
            // 1. Xóa kiểm tra CreatedAt vì chúng ta gán thủ công bên dưới
            ModelState.Remove("CreatedAt");

            if (ModelState.IsValid)
            {
                try
                {
                    booking.CreatedAt = DateTime.Now;
                    _context.Bookings.Add(booking);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Đặt bàn thành công! Seafood sẽ sớm liên hệ với bạn.";
                    return RedirectToAction(nameof(DatBan));
                }
                catch (DbUpdateException ex)
                {
                    // Lỗi này thường do Database chưa được Update Migration
                    System.Diagnostics.Debug.WriteLine($"Lỗi DB: {ex.InnerException?.Message ?? ex.Message}");
                    ModelState.AddModelError("", "Lỗi Database: Hãy chắc chắn bạn đã chạy 'Update-Database'.");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Đã có lỗi xảy ra: " + ex.Message);
                }
            }

            // Nếu dữ liệu không hợp lệ, trả về View kèm thông báo lỗi
            return View(booking);
        }
        // --- TÌM KIẾM ---
        public async Task<IActionResult> Search(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword)) return RedirectToAction("Index");

            var results = await _context.SeafoodItems
                .Where(p => p.Name.Contains(keyword))
                .ToListAsync();

            return View(new SearchViewModel { Keyword = keyword, Results = results });
        }

        [HttpGet]
        public async Task<JsonResult> GetSuggestions(string term)
        {
            if (string.IsNullOrWhiteSpace(term)) return Json(new List<object>());

            var suggestions = await _context.SeafoodItems
                .Where(p => p.Name.Contains(term))
                .Select(p => new { id = p.Id, name = p.Name })
                .Take(5)
                .ToListAsync();

            return Json(suggestions);
        }

        // --- QUẢN LÝ ADMIN ---
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DanhSachDatBan()
        {
            var list = await _context.Bookings.OrderByDescending(x => x.CreatedAt).ToListAsync();
            return View(list);
        }
        public async Task<IActionResult> Index()
        {
            // 1. Lấy 4 sản phẩm để hiện ở phần "Sản phẩm nổi bật"
            // Thử lọc theo danh mục ĐẶC BIỆT trước
            var features = await _context.SeafoodItems
                .Where(x => x.Category == "ĐẶC BIỆT")
                .Take(4)
                .ToListAsync();

            // 2. Nếu danh mục ĐẶC BIỆT trống, hãy lấy đại 4 sản phẩm bất kỳ để trang chủ không bị trắng
            if (features.Count == 0)
            {
                features = await _context.SeafoodItems.Take(4).ToListAsync();
            }

            // 3. Lấy tất cả sản phẩm để dùng cho phần "Thực đơn nhà hàng" bên dưới (nếu bạn muốn hiện hết)
            ViewBag.AllProducts = await _context.SeafoodItems.ToListAsync();

            return View(features); // Truyền features vào Model của View
        }

        // Trang hiển thị Liên hệ
        public async Task<IActionResult> LienHe()
        {
            // Nếu là Admin thì lấy dữ liệu từ Database ra
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                var messages = await _context.ContactMessages
                                             .OrderByDescending(m => m.CreatedDate)
                                             .ToListAsync();
                return View(messages);
            }

            return View(new List<ContactMessage>());
        }
        [HttpPost]
        public async Task<IActionResult> SendContact(ContactMessage model)
        {
            // Kiểm tra xem dữ liệu gửi lên có bị null không
            if (model != null)
            {
                // Gán ngày giờ hiện tại trước khi lưu
                model.CreatedDate = DateTime.Now;

                // Thêm vào DbSet
                _context.ContactMessages.Add(model);

                // QUAN TRỌNG: Lệnh này mới thực sự đẩy dữ liệu xuống SQL Server
                await _context.SaveChangesAsync();

                TempData["Success"] = "Gửi thông tin thành công!";
                return RedirectToAction("LienHe");
            }

            return View("LienHe");
        }
    }

}