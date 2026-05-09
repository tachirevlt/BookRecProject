using System;
using System.Collections.Generic;

namespace Core.Entities
{
    public class BookEntity
    {
        public Guid book_id { get; set; }
        public string authors { get; set; } = null!;
        public double? original_publication_year { get; set; }
        public string original_title { get; set; } = null!;
        public string language_code { get; set; } = null!;
        
        public List<string> tags { get; set; } = new List<string>();

        public int ratings_1 { get; set; } = 0;
        public int ratings_2 { get; set; } = 0;
        public int ratings_3 { get; set; } = 0;
        public int ratings_4 { get; set; } = 0;
        public int ratings_5 { get; set; } = 0;

        public string image_url { get; set; } = null!;
        public string small_image_url { get; set; } = null!;

        // Thuộc tính mới: Giá sách
        public decimal cost { get; set; } = 0; 
    }
}