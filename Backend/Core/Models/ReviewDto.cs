using System;

namespace Application.Models
{
    public class ReviewDto
    {
        public Guid Id { get; set; }
        public Guid user_id { get; set; }
        public string review { get; set; } = null!;
        public DateTime time { get; set; }
    }
}