using System;
using System.Text.Json.Serialization;

namespace Core.Entities
{
    public class UserBookEntity
    {
        public Guid id { get; set; } = Guid.NewGuid();
        public Guid book_id { get; set; }
        public Guid user_id { get; set; }
        public decimal purchase_price { get; set; } = 0;
        public DateTime purchase_date { get; set; } = DateTime.UtcNow;
        public int current_chapter { get; set; } = 0;
        public DateTime? last_read_at { get; set; }

        // Navigation properties
        public virtual BookEntity Book { get; set; } = null!;
        [JsonIgnore]
        public virtual UserEntity User { get; set; } = null!;
    }
}
