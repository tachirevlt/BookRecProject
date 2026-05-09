using System.Collections.Generic;

namespace Core.Models
{
    public class BookRatingResponse
    {
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public List<ReviewDto>? Reviews { get; set; }
    }
}