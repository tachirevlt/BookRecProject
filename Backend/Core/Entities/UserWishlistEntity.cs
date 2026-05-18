using System;
using System.Text.Json.Serialization;

namespace Core.Entities
{
    public class UserWishlistEntity
    {
        public Guid id { get; set; } = Guid.NewGuid();
        public Guid book_id { get; set; }
        public Guid user_id { get; set; }
        public DateTime added_at { get; set; } = DateTime.UtcNow;
        public decimal price_at_addition { get; set; } = 0;
        public string? collection_name { get; set; }

        // Navigation properties
        public virtual BookEntity Book { get; set; } = null!;
        [JsonIgnore]
        public virtual UserEntity User { get; set; } = null!;
    }
}
