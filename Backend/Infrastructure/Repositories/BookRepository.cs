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
                // Khi người dùng chủ động chọn kiểu sắp xếp
                string sortBy = filters.SortBy.Trim().ToLower();
                bool isDescending = filters.SortOrder?.Trim().ToLower() == "desc";

                query = sortBy switch
                {
                    "title" => isDescending ? query.OrderByDescending(b => b.original_title) : query.OrderBy(b => b.original_title),
                    "author" => isDescending ? query.OrderByDescending(b => b.authors) : query.OrderBy(b => b.authors),
                    "year" => isDescending ? query.OrderByDescending(b => b.original_publication_year) : query.OrderBy(b => b.original_publication_year),
                    "rating" => isDescending ? query.OrderByDescending(b => b.ratings_5) : query.OrderBy(b => b.ratings_5), // Lọc theo điểm 5 sao
                    "popularity" => isDescending ? query.OrderByDescending(b => (b.ratings_1 + b.ratings_2 + b.ratings_3 + b.ratings_4 + b.ratings_5)) : query.OrderBy(b => (b.ratings_1 + b.ratings_2 + b.ratings_3 + b.ratings_4 + b.ratings_5)), // Nhiều người đánh giá nhất
                    "price" => isDescending ? query.OrderByDescending(b => b.price) : query.OrderBy(b => b.price),
                    _ => query.OrderBy(b => b.book_id)
                };
            }
            else
            {
                // [TRÁI TIM CỦA BOOKSHELF - PHIÊN BẢN TỐI ƯU CÓ BADGE]
                // Sử dụng sắp xếp đa tầng (Multi-level Sort) thay vì cộng dồn điểm ảo.
                
                query = query
                    // Tầng 1: Tôn trọng Badge (SQL dịch thành CASE WHEN cực kỳ nhẹ và nhanh)
                    .OrderByDescending(b => b.badge == "Trending" ? 3 : 
                                            b.badge == "Hot" ? 2 : 
                                            b.badge == "New" ? 1 : 0)
                    
                    // Tầng 2: Trong cùng một nhóm Badge (hoặc nhóm không có Badge), ưu tiên sách mới
                    // .ThenByDescending(b => b.original_publication_year)
                    
                    // Tầng 3: Cùng năm xuất bản, so kè xem cuốn nào có nhiều lượt 5 sao hơn (Chất lượng)
                    .ThenByDescending(b => b.ratings_5)
                    
                    // Tầng 4: Phân định thắng thua cuối cùng bằng tổng lượt tương tác
                    .ThenByDescending(b => b.ratings_1 + b.ratings_2 + b.ratings_3 + b.ratings_4 + b.ratings_5);
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
                        (b.ratings_2 * -6) - 
                        (b.ratings_1 * -12)
                    )
                )
                
                // 2. Tiêu chí phụ 1: Nếu bằng điểm, ưu tiên nhiều 5 sao hơn
                .ThenByDescending(b => b.ratings_5)
                
                // 3. Tiêu chí phụ 2: Nếu 5 sao cũng bằng nhau, ưu tiên nhiều 4 sao hơn
                .ThenByDescending(b => b.ratings_4)
                
                .Take(count)
                .ToListAsync(ct);
        }
    }
}