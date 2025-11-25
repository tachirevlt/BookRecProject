using System.Collections.Generic;

namespace Application.Models
{
    public class BookRatingResponse
    {
        public double AverageRating { get; set; } // Điểm trung bình
        public int TotalReviews { get; set; }     // Tổng số lượt đánh giá
        public List<ReviewDto>? Reviews { get; set; } // Danh sách chi tiết
    }
}