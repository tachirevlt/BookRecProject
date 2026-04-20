using Core.Entities;
using System.Collections.Generic;
using System;

namespace Core.Models
{
    public class UserDto
    {
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? Email { get; set; } 
        public string Role { get; set; } = string.Empty;
        public string? Sex { get; set; }
        public decimal? CurrentBalance { get; set; }
        public ICollection<BookEntity> FavoriteBooks { get; set; } = new List<BookEntity>();
        public ICollection<BookEntity>? PurchasedBooks { get; set; } = new List<BookEntity>();
    }
}