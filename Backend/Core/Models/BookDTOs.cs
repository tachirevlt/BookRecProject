
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Core.Entities;

namespace Core.Models
{
    // 1. DTO chính cho Sách
    public class BookDTO
    {
        // Bạn nhớ đổi tên biến cho khớp với BookEntity (ví dụ: book_id hay Id)
        public Guid book_id { get; set; }
        public string? authors { get; set; }
        public double? original_publication_year { get; set; }
        public string? original_title { get; set; }
        public string? language_code { get; set; } 
        
        public List<string>? tags { get; set; } 

        public int ratings_1 { get; set; } 
        public int ratings_2 { get; set; } 
        public int ratings_3 { get; set; } 
        public int ratings_4 { get; set; } 
        public int ratings_5 { get; set; } 

        public string image_url { get; set; } = null!;
        public string small_image_url { get; set; } = null!;

        // Giá sách (đổi tên từ cost → price cho khớp CSV)
        public decimal price { get; set; } = 0;

        // Các cột mới bổ sung
        public string? mood { get; set; }
        [JsonIgnore]
        public string? badge { get; set; }
        public string? description { get; set; }
        public string? longDescription { get; set; }
        public int pages { get; set; } 
        public int readTime { get; set; } 
        public string? status { get; set; }
        public int chapters { get; set; } 
        public string? previewText { get; set; }
        public string? accentColor { get; set; }

        // === THỐNG KÊ TƯƠNG TÁC (7 ngày) ===
        [JsonIgnore]
        public int views_7d { get; set; } 
        [JsonIgnore]
        public int favorite_7d { get; set; } 
        [JsonIgnore]
        public int purchases_7d { get; set; } 

        // === THỐNG KÊ TƯƠNG TÁC (30 ngày) ===
        [JsonIgnore]
        public int views_30d { get; set; } 
        [JsonIgnore]
        public int favorite_30d { get; set; } 
        [JsonIgnore]
        public int purchases_30d { get; set; } 

        // === THỐNG KÊ ĐÁNH GIÁ ===
        public int total_ratings { get; set; } 
        public double average_rating { get; set; } 
        
        // === CÁC TRƯỜNG TÍNH TOÁN THÊM ===
        public List<string> badges
        {
            get
            {
                var badges = new List<string>();

                double trendingScore = (purchases_7d * 10) + (favorite_7d * 5) + (views_7d * 1);
                
                if (trendingScore > 350 ) 
                {
                    badges.Add("Trending");
                }

                if (purchases_7d > 10 && badges.Count < 2)
                {
                    badges.Add("Best Seller");
                }

                if (!string.IsNullOrWhiteSpace(badge)&& badges.Count < 2)
                {
                    badges.Add(badge);
                }

                int currentYear = DateTime.UtcNow.Year;
                if (currentYear - original_publication_year <= 2 && badges.Count < 2) 
                {
                    badges.Add("New");
                }

                return badges;
            }
        }

        // Hàm tiện ích để Map từ Entity sang DTO
        public static BookDTO? FromEntity(BookEntity? book)
        {
            if (book == null) return null;
            return new BookDTO
            {
                book_id = book.book_id, 
                authors = book.authors,
                original_title = book.original_title,
                original_publication_year = (int)(book.original_publication_year ?? 2000),
                language_code = book.language_code,
                tags = book.tags,
                ratings_1 = book.ratings_1,
                ratings_2 = book.ratings_2,
                ratings_3 = book.ratings_3,
                ratings_4 = book.ratings_4,
                ratings_5 = book.ratings_5,
                image_url = book.image_url,
                price = (decimal)book.price,
                mood = book.mood,
                badge = book.badge, 
                description = book.description,
                longDescription = book.longDescription,
                pages = book.pages,
                readTime = book.readTime,
                status = book.status,
                chapters = book.chapters,
                previewText = book.previewText,
                accentColor = book.accentColor,
                average_rating = book.average_rating,
                total_ratings = book.total_ratings,
                views_7d = book.views_7d,
                favorite_7d = book.favorite_7d,
                purchases_7d = book.purchases_7d,
                favorite_30d = book.favorite_30d,
                purchases_30d = book.purchases_30d
            };
        }
    }

    // 2. DTO "Mẹ bồng con" cho Wishlist
    public class UserWishlistDTO
    {
        public Guid user_id { get; set; }
        public Guid book_id { get; set; }
        
        public BookDTO? Book { get; set; } // Lồng toàn bộ thông tin sách vào

        public static UserWishlistDTO? FromEntity(UserWishlistEntity? entity)
        {
            if (entity == null) return null;
            return new UserWishlistDTO
            {
                user_id = entity.user_id,
                book_id = entity.book_id,
                Book = BookDTO.FromEntity(entity.Book) // Lưu ý: Repository phải có .Include(w => w.Book)
            };
        }
    }

    // 3. DTO "Mẹ bồng con" cho Tủ sách cá nhân
    public class UserBookDTO
    {
        public Guid user_id { get; set; }
        public Guid book_id { get; set; }
        // public string Status { get; set; } // Ví dụ: "Đang đọc", "Đã xong"
        
        public BookDTO? Book { get; set; } // Lồng toàn bộ thông tin sách vào

        public static UserBookDTO? FromEntity(UserBookEntity? entity)
        {
            if (entity == null) return null;
            return new UserBookDTO
            {
                user_id = entity.user_id,
                book_id = entity.book_id,
                // Status = entity.Status,
                Book = BookDTO.FromEntity(entity.Book)
            };
        }
    }
}