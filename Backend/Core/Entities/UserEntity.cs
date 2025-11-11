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
        public string userName { get; set; } = null!;
        public virtual ICollection<BookEntity> FavoriteBooks { get; set; } = new List<BookEntity>();

    }
}
