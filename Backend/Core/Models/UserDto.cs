using Core.Entities;
using System.Collections.Generic;
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
        public ICollection<BookEntity> FavoriteBooks { get; set; } = new List<BookEntity>();
        public ICollection<BookEntity>? PurchasedBooks { get; set; } = new List<BookEntity>();
    }
}