// File: Backend/Core/Models/UserDto.cs
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
        
        public ICollection<BookEntity> FavoriteBooks { get; set; } = new List<BookEntity>();
    }
}