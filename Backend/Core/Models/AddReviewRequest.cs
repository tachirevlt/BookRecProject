using System;
using System.ComponentModel.DataAnnotations;

namespace Models.Dtos // Bạn có thể đổi namespace thành Core.Models tùy hệ thống
{
    public class AddReviewRequest
    {
        [Required]
        public Guid BookId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập nội dung bình luận.")]
        public string Review { get; set; } = null!;
    }
}