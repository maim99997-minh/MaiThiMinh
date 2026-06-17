using LTWeb.Data;
using LTWeb.Infrastructure;
using LTWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace LTWeb.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private string status;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Lưu giỏ hàng vào Session
        private void SaveCartSession(List<CartItem> cart)
        {
            HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart));
        }

        [HttpPost]
        public IActionResult AddToCart(string id, string name, string image, decimal price, int quantity)
        {
            // 1. Kiểm tra sản phẩm có tồn tại trong DB không
            var product = _context.SeafoodItems.FirstOrDefault(p => p.Id == id);
            if (product == null) return NotFound();

            // 2. Lấy danh sách giỏ hàng hiện tại từ Session
            var cart = GetCartItems();

            // 3. Kiểm tra món này đã có trong giỏ chưa
            // Đảm bảo ProductId trong class CartItem là kiểu string để khớp với id
            var item = cart.FirstOrDefault(p => p.ProductId == id);

            if (item == null)
            {
                // Nếu chưa có thì thêm mới vào list
                cart.Add(new CartItem
                {
                    ProductId = id,
                    ProductName = name,
                    Image = image,
                    Price = price,
                    Quantity = quantity
                });
            }
            else
            {
                // Nếu có rồi thì chỉ tăng số lượng
                item.Quantity += quantity;
            }

            // 4. Lưu lại danh sách vào Session (phải dùng JsonConvert)
            HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart));

            // 5. Chuyển hướng đến trang giỏ hàng
            // Đảm bảo bạn đã tạo file Views/Cart/Index.cshtml
            return RedirectToAction("Index", "Cart");
        }
        public IActionResult Index()
        {
            var cart = GetCartItems(); // Hàm lấy dữ liệu từ Session
            return View(cart);
        }
        // Hàm bổ trợ lấy giỏ hàng từ Session
        private List<CartItem> GetCartItems()
        {
            var sessionCart = HttpContext.Session.GetString("Cart");
            if (string.IsNullOrEmpty(sessionCart))
            {
                return new List<CartItem>();
            }
            return JsonConvert.DeserializeObject<List<CartItem>>(sessionCart);
        }
        public IActionResult Remove(string id)
        {
            // Lấy danh sách giỏ hàng hiện tại từ Session
            var cart = GetCartItems();
            // Tìm sản phẩm có ProductId khớp với id truyền vào
            // Sử dụng .ToString() nếu ProductId trong Model là kiểu int để tránh lỗi CS0019
            var item = cart.FirstOrDefault(p => p.ProductId.ToString() == id);

            if (item != null)
            {
                cart.Remove(item);
                // Cập nhật lại Session sau khi xóa
                HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart));
            }

            // Quay lại trang giỏ hàng để cập nhật giao diện
            return RedirectToAction("Index");
        }
        public IActionResult Checkout()
        {
            // Lấy danh sách giỏ hàng từ Session (giả sử bạn dùng List<CartItem>)
            var cart = HttpContext.Session.GetJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            if (cart.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            // Tính tổng tiền để hiển thị
            ViewBag.Total = cart.Sum(s => s.Quantity * s.Price);

            return View(cart);
        }
        [HttpPost]
        public async Task<IActionResult> ProcessCheckout(Order order)
        {
            var cart = GetCartItems();
            if (cart.Count == 0) return RedirectToAction("Index");

            // Gán giá trị mặc định cho các cột bắt buộc
            order.OrderDate = DateTime.Now;

            // ĐÂY LÀ DÒNG QUAN TRỌNG NHẤT ĐỂ SỬA LỖI TRONG ẢNH:
            order.Status = "Chờ duyệt";

            order.Note = string.IsNullOrEmpty(order.Note) ? "Không có ghi chú" : order.Note;

            if (User.Identity.IsAuthenticated)
            {
                order.CustomerId = User.Identity.Name;
                order.Email = User.Identity.Name;
            }

            order.TotalAmount = cart.Sum(x => x.Quantity * x.Price);

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
        

            // 4. LƯU CHI TIẾT CÁC MÓN ĂN VÀO DATABASE
            foreach (var item in cart)
            {
                var orderDetail = new OrderDetail
                {
                    OrderId = order.OrderId, // Lấy mã đơn thật vừa được DB cấp
                    ProductId = item.ProductId.ToString(),
                    Quantity = item.Quantity,
                    Price = item.Price
                };
                _context.OrderDetails.Add(orderDetail);
            }
            await _context.SaveChangesAsync();

            // 5. Chuẩn bị dữ liệu hiển thị cho trang OrderSuccess
            var confirmation = new OrderConfirmationViewModel
            {
                OrderId = order.OrderId, // Hiển thị mã đơn thật
                CustomerName = $"{order.LastName} {order.FirstName}",
                TotalAmount = cart.Sum(x => x.Quantity * x.Price),
                OrderItems = cart
            };

            // 6. Xóa giỏ hàng sau khi đặt thành công
            HttpContext.Session.Remove("Cart");

            // 7. Chuyển sang trang thông báo thành công
            return View("OrderSuccess", confirmation);
        }
        [HttpPost]
        // Tham số 'status' được khai báo ở đây để nhận giá trị từ giao diện truyền về
        public async Task<IActionResult> UpdateOrderStatus(int orderId, string status)
        {
            // Tìm đơn hàng cần cập nhật
            var order = await _context.Orders.FindAsync(orderId);

            if (order != null)
            {
                // Gán giá trị từ biến 'status' vào cột Status trong Database
                order.Status = status;

                _context.Update(order);
                await _context.SaveChangesAsync();

                TempData["Message"] = "Cập nhật trạng thái thành công!";
            }

            return RedirectToAction("Details", "Admin", new { id = orderId });
        }
        [HttpPost]
        [Route("Admin/UpdateStatus")] // Đường dẫn này để bạn dùng trong thẻ <form>
        public async Task<IActionResult> UpdateStatus(int orderId, string status)
        {
            // Tìm đơn hàng bằng ID
            var order = await _context.Orders.FindAsync(orderId);

            if (order != null)
            {
                // Biến 'status' này lấy từ thẻ <select> của Admin truyền về
                order.Status = status;

                _context.Update(order);
                await _context.SaveChangesAsync();
            }

            // Quay lại trang chi tiết đơn hàng (giả sử trang đó ở Admin/Details)
            return RedirectToAction("Details", "Admin", new { id = orderId });
        }
    }
}