using System.ComponentModel.DataAnnotations;

namespace LTWeb.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên tài khoản hoặc email")]
        [Display(Name = "Tên tài khoản hoặc địa chỉ email")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [Display(Name = "Ghi nhớ mật khẩu")]
        public bool RememberMe { get; set; }
    }
}