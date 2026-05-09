using System;
using System.ComponentModel.DataAnnotations;

namespace Models.Dtos
{
    public class AddRatingRequest
    {
        [Required]
        public Guid book_id { get; set; }

        [Range(1, 5, ErrorMessage = "Điểm đánh giá phải từ 1 đến 5 sao.")]
        public int Rating { get; set; }
    }
}