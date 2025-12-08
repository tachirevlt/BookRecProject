using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    public class ChangePasswordDto
    {
        // Bắt buộc phải có mật khẩu cũ để xác thực
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu hiện tại.")]
        public string CurrentPassword { get; set; }= string.Empty;

        // Bắt buộc nhập mật khẩu mới + Báo lỗi nếu để trống
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới.")]
        [MinLength(6, ErrorMessage = "Mật khẩu mới phải có ít nhất 6 ký tự.")]
        public string NewPassword { get; set; }= string.Empty;
    }
}