using Microsoft.EntityFrameworkCore;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Persistence;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly AppDbContext _db;

        public BookRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<BookEntity?> GetBookByIdAsync(Guid id, CancellationToken ct = default)
        {
            var book = await _db.Books.FindAsync(new object?[] { id }, ct);
            if (book == null) throw new KeyNotFoundException($"Không tìm thấy sách với ID: {id}");
            return book;
        }

        public async Task<(IReadOnlyList<BookEntity> Books, int TotalCount)> GetAllBooksWithPaginationAndFilteringAsync(
            PaginationParams pagination,
            BookFilterParams filters,
            CancellationToken cancellationToken)
        {
            IQueryable<BookEntity> query = _db.Books.AsNoTracking();
            
            // Lấy năm hiện tại 1 lần ở ngoài để SQL dễ dàng biên dịch
            int currentYear = DateTime.UtcNow.Year; 

            // 1. CÁC BỘ LỌC TÌM KIẾM CƠ BẢN
            if (!string.IsNullOrWhiteSpace(filters.SearchTerm))
            {
                var term = filters.SearchTerm.Trim().ToLower();
                query = query.Where(b => b.original_title.ToLower().Contains(term) || 
                                        b.authors.ToLower().Contains(term) ||
                                        b.tags.Contains(term));
            }

            if (!string.IsNullOrWhiteSpace(filters.Title))
            {
                var titleFilter = filters.Title.Trim().ToLower();
                query = query.Where(b => b.original_title.ToLower().Contains(titleFilter));
            }

            if (!string.IsNullOrWhiteSpace(filters.Author))
            {
                var authorFilter = filters.Author.Trim().ToLower();
                query = query.Where(b => b.authors.ToLower().Contains(authorFilter));
            }

            if (!string.IsNullOrWhiteSpace(filters.Genre))
            {
                var genreFilter = filters.Genre.Trim().ToLower();
                query = query.Where(b => b.tags.Contains(genreFilter));
            }

            
            // 2. LỌC THEO SỐ ĐIỂM 
            if (filters.MinRating.HasValue || filters.MaxRating.HasValue)
            {
                double minRating = (double)(filters.MinRating ?? 0m);
                double maxRating = (double)(filters.MaxRating ?? 5m);

                query = query.Where(b => 
                    // Vẫn phải có ít nhất 1 đánh giá
                    (b.ratings_1 + b.ratings_2 + b.ratings_3 + b.ratings_4 + b.ratings_5) > 0 &&
                    
                    // Tổng điểm >= Min * Tổng số lượt đánh giá
                    (double)(b.ratings_1 * 1 + b.ratings_2 * 2 + b.ratings_3 * 3 + b.ratings_4 * 4 + b.ratings_5 * 5) 
                    >= minRating * (double)(b.ratings_1 + b.ratings_2 + b.ratings_3 + b.ratings_4 + b.ratings_5) &&
                    
                    // Tổng điểm <= Max * Tổng số lượt đánh giá
                    (double)(b.ratings_1 * 1 + b.ratings_2 * 2 + b.ratings_3 * 3 + b.ratings_4 * 4 + b.ratings_5 * 5) 
                    <= maxRating * (double)(b.ratings_1 + b.ratings_2 + b.ratings_3 + b.ratings_4 + b.ratings_5)
                );
            }

            if (filters.MinYear.HasValue)
                query = query.Where(b => b.original_publication_year >= filters.MinYear.Value);
                
            if (filters.MaxYear.HasValue)
                query = query.Where(b => b.original_publication_year <= filters.MaxYear.Value);

            // 3. THUẬT TOÁN SẮP XẾP (SORTING)
            if (!string.IsNullOrWhiteSpace(filters.SortBy))
            {
                string sortBy = filters.SortBy.Trim().ToLower();
                bool isDescending = filters.SortOrder?.Trim().ToLower() == "desc";

                query = sortBy switch
                {
                    "title" => isDescending ? query.OrderByDescending(b => b.original_title) : query.OrderBy(b => b.original_title),
                    "author" => isDescending ? query.OrderByDescending(b => b.authors) : query.OrderBy(b => b.authors),
                    "year" => isDescending ? query.OrderByDescending(b => b.original_publication_year) : query.OrderBy(b => b.original_publication_year),
                    "rating" => isDescending ? query.OrderByDescending(b => b.ratings_5) : query.OrderBy(b => b.ratings_5),
                    "popularity" => isDescending ? query.OrderByDescending(b => b.total_ratings) : query.OrderBy(b => b.total_ratings),
                    "price" => isDescending ? query.OrderByDescending(b => b.price) : query.OrderBy(b => b.price),
                    
                    // Tối ưu thuật toán Trending đồng nhất với BadgeService
                    "trending_7d" => isDescending 
                        ? query.OrderByDescending(b => (b.purchases_7d * 10) + (b.favorite_7d * 5) + b.views_7d) 
                        : query.OrderBy(b => (b.purchases_7d * 10) + (b.favorite_7d * 5) + b.views_7d),
                    
                    "trending_30d" => isDescending 
                        ? query.OrderByDescending(b => (b.purchases_30d * 10) + (b.favorite_30d * 5) + b.views_30d) 
                        : query.OrderBy(b => (b.purchases_30d * 10) + (b.favorite_30d * 5) + b.views_30d),

                    _ => query.OrderBy(b => b.book_id)
                };
            }
            else
            {
                // ĐÃ SỬA: SẮP XẾP DEFAULT THEO ĐÚNG CẤP ĐỘ ƯU TIÊN CỦA BADGE
                query = query
                    // Ưu tiên 1: Sách có nhãn thủ công (True xếp trước False)
                    .OrderByDescending(b => b.badge != null && b.badge != "")
                    
                    // Ưu tiên 2: Best Seller (Căn cứ theo số lượt mua cao nhất)
                    .ThenByDescending(b => b.purchases_7d)
                    
                    // Ưu tiên 3: Trending (Sách nào thoả mãn công thức Trending sẽ được ưu tiên nổi lên)
                    .ThenByDescending(b => 
                        ((b.purchases_7d * 10) + (b.favorite_7d * 5) + b.views_7d) > 50 &&
                        ((b.purchases_7d * 10) + (b.favorite_7d * 5) + b.views_7d) > (((b.purchases_30d * 10) + (b.favorite_30d * 5) + b.views_30d) / 4.0 * 1.8)
                    )
                    // Phụ trợ 3.1: Nếu cả 2 sách đều thoả Trending, quyển nào điểm cao hơn xếp trên
                    .ThenByDescending(b => (b.purchases_7d * 10) + (b.favorite_7d * 5) + b.views_7d)
                    
                    // Ưu tiên 4: New (Sách xuất bản <= 10 năm)
                    .ThenByDescending(b => b.original_publication_year != null && (currentYear - b.original_publication_year) <= 10)
                    
                    // Ưu tiên 5: Điểm đánh giá (Sách hay tự động nổi lên)
                    .ThenByDescending(b => b.ratings_5)
                    .ThenByDescending(b => b.total_ratings);
            }
            
            // 4. THỰC THI PHÂN TRANG
            int totalCount = await query.CountAsync(cancellationToken);

            var books = await query
                .Skip((pagination.PageNumber - 1) * pagination.PageSize) 
                .Take(pagination.PageSize) 
                .ToListAsync(cancellationToken);

            return (books, totalCount);
        }
        public async Task<BookEntity> AddBookAsync(BookEntity book, CancellationToken ct = default)
        {
            await _db.Books.AddAsync(book, ct);
            await _db.SaveChangesAsync(ct);
            return book;
        }

        public async Task<BookEntity> UpdateBookAsync(Guid bookId, BookEntity updatedBookData, CancellationToken ct = default)
        {
            var existingBook = await _db.Books.FindAsync(new object?[] { bookId }, ct);
            if (existingBook is null)
            {
                throw new KeyNotFoundException($"Không tìm thấy sách với ID: {bookId}");
            }

            existingBook.original_title = updatedBookData.original_title;
            existingBook.authors = updatedBookData.authors;
            existingBook.tags = updatedBookData.tags;
            existingBook.original_publication_year = updatedBookData.original_publication_year;
            existingBook.language_code = updatedBookData.language_code;
            existingBook.image_url = updatedBookData.image_url;
            existingBook.small_image_url = updatedBookData.small_image_url;
            existingBook.price = updatedBookData.price;
            existingBook.mood = updatedBookData.mood;
            existingBook.badge = updatedBookData.badge;
            existingBook.description = updatedBookData.description;
            existingBook.longDescription = updatedBookData.longDescription;
            existingBook.pages = updatedBookData.pages;
            existingBook.readTime = updatedBookData.readTime;
            existingBook.status = updatedBookData.status;
            existingBook.chapters = updatedBookData.chapters;
            existingBook.previewText = updatedBookData.previewText;
            existingBook.accentColor = updatedBookData.accentColor;
            // Không nên update trực tiếp ratings_1 -> 5 ở đây, vì rating cập nhật qua AddRatingCommand

            _db.Books.Update(existingBook);
            await _db.SaveChangesAsync(ct);
            return existingBook;
        }

        public async Task<bool> DeleteBookAsync(Guid id, CancellationToken ct = default)
        {
            var entity = await _db.Books.FindAsync(new object?[] { id }, ct);
            if (entity is null) return false;
            
            _db.Books.Remove(entity);
            await _db.SaveChangesAsync(ct);
            return true;
        }

        public async Task<IReadOnlyList<BookEntity>> GetRecommendedBooksAsync(int count, CancellationToken ct = default)
        {
            return await _db.Books.AsNoTracking()
                // Đã xóa phần Where loại trừ book_id
                
                // 1. Sắp xếp chính: Dựa trên tổng điểm Badge + Điểm Rating
                .OrderByDescending(b => 
                    (
                        b.badge == "Trending" ? 100 :
                        b.badge == "Hot" ? 90 :
                        b.badge == "Bestseller" ? 80 :
                        b.badge == "Editor's Choice" ? 60 :
                        b.badge == "New" ? 40 : 
                        0
                    ) 
                    + 
                    (
                        (b.ratings_5 * 10) + 
                        (b.ratings_4 * 4) + 
                        (b.ratings_3 * -2) - 
                        (b.ratings_2 * 6) - 
                        (b.ratings_1 * 12)
                    )
                )
                
                // 2. Tiêu chí phụ 1: Nếu bằng điểm, ưu tiên nhiều 5 sao hơn
                .ThenByDescending(b => b.ratings_5)
                
                // 3. Tiêu chí phụ 2: Nếu 5 sao cũng bằng nhau, ưu tiên nhiều 4 sao hơn
                .ThenByDescending(b => b.ratings_4)
                
                .Take(count)
                .ToListAsync(ct);
        }
        public async Task<List<BookEntity>> GetAllBooksAsync(CancellationToken ct = default)
        {
            // Không dùng AsNoTracking ở đây vì Worker cần Update lại danh sách này
            return await _db.Books.ToListAsync(ct);
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _db.SaveChangesAsync(ct);
        }
    }
}