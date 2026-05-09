using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    public class UserLoginDto
    {
        [Required]
        public string user_name { get; set; } = null!;
        
        [Required]
        public string Password { get; set; } = null!; 
    }
}