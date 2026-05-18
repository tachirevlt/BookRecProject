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

        // Giá sách (đổi tên từ cost → price cho khớp CSV)
        public decimal price { get; set; } = 0;

        // Các cột mới bổ sung
        public string? mood { get; set; }
        public string? badge { get; set; }
        public string? description { get; set; }
        public string? longDescription { get; set; }
        public int pages { get; set; } = 0;
        public int readTime { get; set; } = 0;
        public string? status { get; set; }
        public int chapters { get; set; } = 0;
        public string? previewText { get; set; }
        public string? accentColor { get; set; }

        // === THỐNG KÊ TƯƠNG TÁC (7 ngày) ===
        public int views_7d { get; set; } = 0;
        public int favorite_7d { get; set; } = 0;
        public int purchases_7d { get; set; } = 0;

        // === THỐNG KÊ TƯƠNG TÁC (30 ngày) ===
        public int views_30d { get; set; } = 0;
        public int favorite_30d { get; set; } = 0;
        public int purchases_30d { get; set; } = 0;

        // === THỐNG KÊ ĐÁNH GIÁ ===
        public int total_ratings { get; set; } = 0;
        public double average_rating { get; set; } = 0.0;
    }
}