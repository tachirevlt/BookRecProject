using System;

namespace Application.Models
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public Guid UserId { get; set; } 
        // Đã loại bỏ ratings_1 đến ratings_5
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}