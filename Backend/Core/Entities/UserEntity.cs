using System;
using System.Collections.Generic;

namespace Core.Entities
{
    public class UserEntity
    {
        public Guid UserId { get; set; }     
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string HashedPassword { get; set; } = null!;
        public string Role { get; set; } = null!;
        
        public string? Sex { get; set; } 
        public decimal CurrentBalance { get; set; } = 0;

        public virtual ICollection<BookEntity> FavoriteBooks { get; set; } = new List<BookEntity>();
        
        // Danh sách sách đã mua
        public virtual ICollection<BookEntity> PurchasedBooks { get; set; } = new List<BookEntity>();
    }
}