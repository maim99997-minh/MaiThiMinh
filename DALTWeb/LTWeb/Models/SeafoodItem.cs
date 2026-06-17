using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LTWeb.Models
{
    public class SeafoodItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; } // Nên để kiểu string nếu ID của bạn là chuỗi

        [Required(ErrorMessage = "Vui lòng nhập tên món ăn")]
        [Display(Name = "Tên món hải sản")]
        public string Name { get; set; }

        [Display(Name = "Mô tả")]
        public string Description { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "Giá (VNĐ)")]
        public decimal Price { get; set; }

        // Sửa ImageUrl thành Image để khớp với View đang dùng @Model.Image
        [Display(Name = "Đường dẫn ảnh")]
        public string Image { get; set; }

        public string Category { get; set; }
        public string Origin { get; set; }
        public string Weight { get; set; }
    }
}