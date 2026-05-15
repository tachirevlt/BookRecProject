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

        public virtual ICollection<BookEntity> FavoriteBooks { get; set; } = new List<BookEntity>();
        
        public virtual ICollection<BookEntity> PurchasedBooks { get; set; } = new List<BookEntity>();
    }
}