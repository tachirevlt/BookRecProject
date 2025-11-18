// Đã sửa: Core/Entities/UserEntity.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class UserEntity
    {
        public Guid UserId { get; set; }     
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string HashedPassword { get; set; } = null!;
        public string Role { get; set; } = null!;

        public virtual ICollection<BookEntity> FavoriteBooks { get; set; } = new List<BookEntity>();
    }
}