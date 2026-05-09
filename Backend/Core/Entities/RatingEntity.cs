using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public class RatingEntity
    {
        [Required]
        public Guid user_id { get; set; }
        [ForeignKey("user_id")]
        public UserEntity? User { get; set; }

        [Required]
        public Guid book_id { get; set; }
        [ForeignKey("book_id")]
        public BookEntity? Book { get; set; }

        [Range(1, 5)]
        public int rating { get; set; }

        public DateTime time { get; set; } = DateTime.UtcNow;
    }
}