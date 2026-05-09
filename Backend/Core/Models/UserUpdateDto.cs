using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    public class UserUpdateDto
    {
        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        public string user_name { get; set; } = string.Empty;
        [Required(ErrorMessage = "email không được để trống")]
        [EmailAddress(ErrorMessage = "email không hợp lệ")]
        public string email { get; set; } = string.Empty;
    }
}