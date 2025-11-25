using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public class ReviewEntity
    {
        [Key]
        public int Id { get; set; } // ID của Review giữ là int cũng được, hoặc đổi sang Guid tùy bạn

        [Required]
        public Guid UserId { get; set; } // SỬA: int -> Guid
        [ForeignKey("UserId")]
        public UserEntity? User { get; set; }

        [Required]
        public Guid BookId { get; set; } // SỬA: int -> Guid
        [ForeignKey("BookId")]
        public BookEntity? Book { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}