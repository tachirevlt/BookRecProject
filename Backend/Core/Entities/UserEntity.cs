using System;
using System.Collections.Generic;

namespace Core.Entities
{
    public class UserEntity
    {
        public Guid user_id { get; set; }     
        public string user_name { get; set; } = null!;
        public string email { get; set; } = null!;
        public string hashed_password { get; set; } = null!;
        public string full_name { get; set; } = string.Empty;
        public string role { get; set; } = null!;
        
        public string? sex { get; set; } 
        public decimal current_balance { get; set; } = 0;

        // Navigation properties — quản lý qua UserWishlistEntity và UserBookEntity
        public virtual ICollection<UserWishlistEntity> Wishlists { get; set; } = new List<UserWishlistEntity>();
        public virtual ICollection<UserBookEntity> UserBooks { get; set; } = new List<UserBookEntity>();
    }
}