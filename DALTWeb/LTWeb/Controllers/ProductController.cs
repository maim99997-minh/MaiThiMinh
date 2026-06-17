using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LTWeb.Data;
using LTWeb.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace LTWeb.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- CREATE ---
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SeafoodItem item)
        {
            if (ModelState.IsValid)
            {
                _context.SeafoodItems.Add(item);
                await _context.SaveChangesAsync();
                return RedirectToAction("Menu", "Home");
            }
            return View(item);
        }

        // --- EDIT (Hiển thị Form) ---
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();
            var item = await _context.SeafoodItems.FindAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        // --- EDIT (Xử lý lưu vào SQL) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, SeafoodItem item)
        {
            if (id != item.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(item);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.SeafoodItems.Any(e => e.Id == item.Id)) return NotFound();
                    else throw;
                }
                // Sau khi sửa, quay về trang chi tiết của chính món đó trong MenuController
                return RedirectToAction("Details", "Menu", new { id = item.Id });
            }
            return View(item);
        }

        // --- DELETE (Xóa khỏi SQL) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var item = await _context.SeafoodItems.FindAsync(id);
            if (item != null)
            {
                _context.SeafoodItems.Remove(item);
                await _context.SaveChangesAsync();
            }
            // Xóa xong quay về trang Menu chính
            return RedirectToAction("Menu", "Home");
        }
    }
}