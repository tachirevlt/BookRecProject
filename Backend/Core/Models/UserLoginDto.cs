using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    // Lớp này chỉ dùng để nhận JSON cho việc đăng nhập
    public class UserLoginDto
    {
        [Required]
        public string Username { get; set; } = null!;
        
        [Required]
        public string Password { get; set; } = null!; 
    }
}