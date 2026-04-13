using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class BookEntity
    {
        public Guid BookId { get; set; }
        public string title { get; set; } = null!;
        public string author { get; set; } = null!;
        public List<string> Genres { get; set; } = new List<string>();
        public double? year { get; set; } = null!;
        public int? books_count { get; set; } = null!;
        public string work_id { get; set; } = null!;
        public string isbn { get; set; } = null!;
        public string language_code { get; set; } = null!;
        public decimal? average_rating { get; set; } = null!;
        public int? ratings { get; set; } = null!;
    }
}
// 'id', 'book_id', 'authors', 'original_publication_year',
//        'original_title', 'language_code', 'ratings_1', 'ratings_2',
//        'ratings_3', 'ratings_4', 'ratings_5', 'image_url', 'small_image_url'