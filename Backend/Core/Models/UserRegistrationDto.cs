// File: Backend/Core/Models/UserRegistrationDto.cs
using System.ComponentModel.DataAnnotations; // Cần thư viện này

namespace Core.Models
{
    public class UserRegistrationDto
    {
        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc.")]
        public string user_name { get; set; } = string.Empty;

        [Required(ErrorMessage = "email là bắt buộc.")]
        [EmailAddress(ErrorMessage = "Định dạng email không hợp lệ.")]
        public string email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tên đầy đủ là bắt buộc.")]
        public string full_name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
        public string Password { get; set; } = string.Empty;
        [Required(ErrorMessage = "Vui lòng chọn giới tính")]
        [RegularExpression("^(Male|Female|Other)$", ErrorMessage = "Giới tính chỉ được nhập 'Nam', 'Nữ' hoặc 'Khác'")]
        public string? sex { get; set; }
    }
}