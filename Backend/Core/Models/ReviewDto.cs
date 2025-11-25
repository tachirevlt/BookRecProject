using System;

namespace Application.Models
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public Guid UserId { get; set; } 
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}