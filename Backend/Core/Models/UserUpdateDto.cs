using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    public class UserUpdateDto
    {
        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = string.Empty;

        public string? Password { get; set; } 
    }
}