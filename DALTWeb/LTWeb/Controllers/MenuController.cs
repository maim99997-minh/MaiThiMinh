using LTWeb.Data;
using LTWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LTWeb.Controllers
{
    public class MenuController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MenuController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Trang 1: CÁC LOẠI HẢI SẢN KHÁC
        public async Task<IActionResult> Index()
        {
            var items = await _context.SeafoodItems
                .Where(x => x.Category == "CÁC LOẠI HẢI SẢN KHÁC")
                .ToListAsync();
            return View("~/Views/Home/Menu.cshtml", items);
        }

        // Trang 2: CA
        public async Task<IActionResult> Menu2()
        {
            var items = await _context.SeafoodItems
                .Where(x => x.Category == "CA")
                .ToListAsync();
            return View("~/Views/Home/Menu2.cshtml", items);
        }

        // Trang 3: CA (Đặc biệt)
        public async Task<IActionResult> Menu3()
        {
            var items = await _context.SeafoodItems
                .Where(x => x.Category == "ĐẶC BIỆT")
                .ToListAsync();
            return View("~/Views/Home/Menu3.cshtml", items);
        }

        // Trang 4: CUA - GHẸ
        public async Task<IActionResult> Menu4()
        {
            var items = await _context.SeafoodItems
                .Where(x => x.Category == "CUA - GHE")
                .ToListAsync();
            return View("~/Views/Home/Menu4.cshtml", items);
        }

        // Trang 5: NGHEU - SO - OC
        public async Task<IActionResult> Menu5()
        {
            var items = await _context.SeafoodItems
                .Where(x => x.Category == "NGHEU - SO - OC")
                .ToListAsync();
            return View("~/Views/Home/Menu5.cshtml", items);
        }

        // Link chi tiết (Details)
        public async Task<IActionResult> Details(string id)
        {
            if (id == null) return NotFound();
            var item = await _context.SeafoodItems.FirstOrDefaultAsync(m => m.Id == id);
            if (item == null) return NotFound();
            return View(item);
        }

        // Trang thêm mới (chỉ Admin)
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price,Category,ImageUrl,Origin,Weight,Description")] SeafoodItem item)
        {
            if (ModelState.IsValid)
            {
                _context.Add(item);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(item);
        }
    }
}