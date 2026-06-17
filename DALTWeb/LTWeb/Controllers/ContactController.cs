using LTWeb.Data;
using LTWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LTWeb.Controllers
{
    public class ContactController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContactController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Contact (Trang liên hệ cho khách)
        public IActionResult Index()
        {
            return View();
        }

        // POST: Contact/Send
        [HttpPost]
        public async Task<IActionResult> Send(ContactMessage model)
        {
            if (ModelState.IsValid)
            {
                _context.ContactMessages.Add(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Gửi tin nhắn thành công!";
                return RedirectToAction("Index");
            }
            return View("Index", model);
        }

        // GET: Contact/AdminList (Chỉ Admin xem được)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminList()
        {
            var messages = await _context.ContactMessages.OrderByDescending(m => m.CreatedDate).ToListAsync();
            return View(messages);
        }
    }
}
