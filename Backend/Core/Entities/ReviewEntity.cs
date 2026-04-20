using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public class ReviewEntity
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid user_id { get; set; }
        [ForeignKey("user_id")]
        public UserEntity? User { get; set; }

        [Required]
        public Guid book_id { get; set; }
        [ForeignKey("book_id")]
        public BookEntity? Book { get; set; }

        [Required]
        public string review { get; set; } = null!;

        public DateTime time { get; set; } = DateTime.UtcNow;
    }
}