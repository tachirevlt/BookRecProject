using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    public class AddReviewRequest
    {
        [Required]
        public Guid book_id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập nội dung bình luận.")]
        public string Review { get; set; } = null!;
    }
}