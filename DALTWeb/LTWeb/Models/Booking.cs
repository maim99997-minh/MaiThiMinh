using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LTWeb.Models
{
    public class Booking
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên khách hàng")]
        [StringLength(100)]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@gmail\.com$",
            ErrorMessage = "Hệ thống chỉ chấp nhận địa chỉ Gmail (ví dụ: ten@gmail.com)")]
        public string Email { get; set; } // <--- Tớ đã thêm trường này

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không đúng định dạng")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn chi nhánh")]
        public string Branch { get; set; } // <--- Tớ đã thêm trường này

        [Required(ErrorMessage = "Vui lòng chọn ngày giờ đặt bàn")]
        public DateTime BookingDate { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số người")]
        [Range(1, 100, ErrorMessage = "Số lượng người từ 1 đến 100")]
        public int NumberOfPeople { get; set; }

        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}