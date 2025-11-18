// File: Backend/Core/Models/UserRegistrationDto.cs
using System.ComponentModel.DataAnnotations; // Cần thư viện này

namespace Core.Models
{
    public class UserRegistrationDto
    {
        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email là bắt buộc.")]
        [EmailAddress(ErrorMessage = "Định dạng email không hợp lệ.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
        public string Password { get; set; } = string.Empty;
    }
}