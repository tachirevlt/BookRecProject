using Microsoft.EntityFrameworkCore;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Persistence;
using Core.Models;

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
                if (!string.IsNullOrWhiteSpace(filters.SearchTerm))
                {
                    var term = filters.SearchTerm.Trim().ToLower();
                    query = query.Where(b => b.title.ToLower().Contains(term) || 
                                            b.author.ToLower().Contains(term) ||
                                            // 👇 Logic tìm trong List<string>
                                            b.Genres.Any(g => g.ToLower().Contains(term)));
                }

                if (!string.IsNullOrWhiteSpace(filters.Title))
                {
                    query = query.Where(b => b.title.Contains(filters.Title));
                }

                if (!string.IsNullOrWhiteSpace(filters.Author))
                {
                    query = query.Where(b => b.author.Contains(filters.Author));
                }

                if (!string.IsNullOrWhiteSpace(filters.Genre))
                {
                    var genreTerm = filters.Genre.Trim();
                    query = query.Where(b => b.Genres.Contains(genreTerm));
                }
                
                
                if (filters.MinYear.HasValue)
                {
                    query = query.Where(b => b.year >= filters.MinYear.Value);
                }
                
                if (filters.MaxYear.HasValue)
                {
                    query = query.Where(b => b.year <= filters.MaxYear.Value);
                }
                
                
                query = query.OrderBy(b => b.BookId); 

                if (!string.IsNullOrWhiteSpace(filters.SortBy))
                {
                    string sortBy = filters.SortBy.ToLowerInvariant();
                    
                    bool isDescending = filters.SortOrder?.Equals("desc", StringComparison.OrdinalIgnoreCase) == true;

                    query = sortBy switch
                    {
                        "title" => isDescending ? query.OrderByDescending(b => b.title) : query.OrderBy(b => b.title),
                        "author" => isDescending ? query.OrderByDescending(b => b.author) : query.OrderBy(b => b.author),
                        "year" => isDescending ? query.OrderByDescending(b => b.year) : query.OrderBy(b => b.year),
                        _ => query 
                    };
                }
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

            existingBook.title = updatedBookData.title;
            existingBook.author = updatedBookData.author;
            existingBook.Genres = updatedBookData.Genres;
            existingBook.year = updatedBookData.year;
            existingBook.books_count = updatedBookData.books_count;
            existingBook.work_id = updatedBookData.work_id;
            existingBook.isbn = updatedBookData.isbn;
            existingBook.language_code = updatedBookData.language_code;
            existingBook.average_rating = updatedBookData.average_rating;
            existingBook.ratings_1 = updatedBookData.ratings_1;
            existingBook.ratings_2 = updatedBookData.ratings_2;
            existingBook.ratings_3 = updatedBookData.ratings_3;
            existingBook.ratings_4 = updatedBookData.ratings_4;
            existingBook.ratings_5 = updatedBookData.ratings_5;

            _db.Books.Update(existingBook);
            await _db.SaveChangesAsync(ct);
            return existingBook;
        }

        public async Task<bool> DeleteBookAsync(Guid id, CancellationToken ct = default)
        {
            var entity = await _db.Books.FindAsync(new object?[] { id }, ct);
            if (entity is null)
            {
                return false;
            }
            _db.Books.Remove(entity);
            await _db.SaveChangesAsync(ct);
            return true;
        }
        public async Task IncrementRatingCountAsync(Guid bookId, int rating, CancellationToken ct = default)
        {
            var book = await _db.Books.FindAsync(new object?[] { bookId }, ct);
            
            if (book is null)
            {
                throw new KeyNotFoundException($"Không tìm thấy sách với ID: {bookId} để cập nhật đánh giá.");
            }

            // 1. Tăng số lượng đánh giá tương ứng (ratings_1 đến ratings_5)
            switch (rating)
            {
                case 1:
                    book.ratings_1 = (book.ratings_1 ?? 0) + 1;
                    break;
                case 2:
                    book.ratings_2 = (book.ratings_2 ?? 0) + 1;
                    break;
                case 3:
                    book.ratings_3 = (book.ratings_3 ?? 0) + 1;
                    break;
                case 4:
                    book.ratings_4 = (book.ratings_4 ?? 0) + 1;
                    break;
                case 5:
                    book.ratings_5 = (book.ratings_5 ?? 0) + 1;
                    break;
                default:
                    // Bỏ qua nếu rating không hợp lệ (mặc dù AddReviewRequest đã validate từ 1-5)
                    return; 
            }

            // 2. Tính toán lại tổng số lượt vote và điểm trung bình
            
            // Đếm lại tổng số votes (books_count)
            long totalVotes = (book.ratings_1 ?? 0) + 
                              (book.ratings_2 ?? 0) + 
                              (book.ratings_3 ?? 0) + 
                              (book.ratings_4 ?? 0) + 
                              (book.ratings_5 ?? 0);
            
            book.books_count = (int)totalVotes; // Cập nhật tổng số lượt vote

            // Tính tổng điểm
            decimal totalScore = (decimal)((book.ratings_1 ?? 0) * 1) +
                                   (decimal)((book.ratings_2 ?? 0) * 2) +
                                   (decimal)((book.ratings_3 ?? 0) * 3) +
                                   (decimal)((book.ratings_4 ?? 0) * 4) +
                                   (decimal)((book.ratings_5 ?? 0) * 5);

            // Tính điểm trung bình
            if (totalVotes > 0)
            {
                // Sử dụng decimal và làm tròn 2 chữ số thập phân cho độ chính xác
                book.average_rating = Math.Round(totalScore / (decimal)totalVotes, 2); 
            }
            else
            {
                book.average_rating = null;
            }
            
            // 3. Lưu thay đổi vào Database
            await _db.SaveChangesAsync(ct);
        }
    }
}
