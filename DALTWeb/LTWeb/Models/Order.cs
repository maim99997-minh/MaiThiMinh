using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LTWeb.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ")]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên")]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string PhoneNumber { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        public string Note { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        // Trạng thái đơn hàng: Pending, Confirmed, Shipped, Cancelled
        public string Status { get; set; } // Phải là kiểu string

        public string CustomerId { get; set; }

        // Khởi tạo Collection trong Constructor để tránh lỗi Null
        public Order()
        {
            OrderDate = DateTime.Now;
            OrderDetails = new List<OrderDetail>();
        }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}