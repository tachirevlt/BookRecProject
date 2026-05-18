using System;

namespace Core.Models
{
    public class UserDto
    {
        public Guid user_id { get; set; }
        public string user_name { get; set; } = string.Empty;
        public string full_name { get; set; } = string.Empty;
        public string? email { get; set; }
        public string role { get; set; } = string.Empty;
        public string? sex { get; set; }
        public decimal? current_balance { get; set; }
    }
}