using System;

namespace Core.Models
{
    public class ReviewDto
    {
        public Guid id { get; set; }
        public Guid user_id { get; set; }
        public Guid book_id { get; set; }
        public string review { get; set; } = null!;
        public DateTime time { get; set; }
    }
}