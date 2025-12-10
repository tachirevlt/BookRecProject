using System;

namespace Application.Models
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public Guid UserId { get; set; } 
        public int ratings_1 { get; set; }
        public int ratings_2 { get; set; }
        public int ratings_3 { get; set; } 
        public int ratings_4 { get; set; }
        public int ratings_5 { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}