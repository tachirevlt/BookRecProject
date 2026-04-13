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

            // 1. Logic tìm kiếm tổng hợp (SearchTerm)
            if (!string.IsNullOrWhiteSpace(filters.SearchTerm))
            {
                var term = filters.SearchTerm.Trim().ToLower();
                // Tìm trong Title, Author HOẶC bất kỳ Genre nào chứa từ khoá
                query = query.Where(b => b.title.Contains(term) || 
                                         b.author.Contains(term) ||
                                         b.Genres.Contains(term));
            }

            // 2. Các bộ lọc cụ thể
            if (!string.IsNullOrWhiteSpace(filters.Title))
            {
                var titleFilter = filters.Title.Trim().ToLower();
                query = query.Where(b => b.title.ToLower().Contains(titleFilter));
            }

            if (!string.IsNullOrWhiteSpace(filters.Author))
            {
                var authorFilter = filters.Author.Trim().ToLower();
                query = query.Where(b => b.author.ToLower().Contains(authorFilter));
            }

            // 3. Logic lọc Genre chuẩn xác
            if (!string.IsNullOrWhiteSpace(filters.Genre))
            {
                var genreFilter = filters.Genre.Trim().ToLower();
                // Dùng Any() để kiểm tra danh sách Genres của sách có chứa thể loại cần tìm không
                // So sánh chính xác (Equals) hoặc chứa (Contains) tuỳ nhu cầu, ở đây dùng Contains cho linh hoạt
                query = query.Where(b => b.Genres.Contains(genreFilter));
            }
            
            // Các bộ lọc số (Rating, Year)
            if (filters.MinRating.HasValue)
            {
                query = query.Where(b => b.average_rating >= filters.MinRating.Value);
            }

            if (filters.MaxRating.HasValue)
            {
                query = query.Where(b => b.average_rating <= filters.MaxRating.Value);
            }
            
            if (filters.MinYear.HasValue)
            {
                query = query.Where(b => b.year >= filters.MinYear.Value);
            }
            
            if (filters.MaxYear.HasValue)
            {
                query = query.Where(b => b.year <= filters.MaxYear.Value);
            }
            
            // 4. Sắp xếp (Sorting)
            // Mặc định sắp xếp theo BookId để đảm bảo thứ tự phân trang ổn định
            query = query.OrderBy(b => b.BookId); 

            if (!string.IsNullOrWhiteSpace(filters.SortBy))
            {
                string sortBy = filters.SortBy.Trim().ToLower(); // Trim ở đây cho chắc chắn
                bool isDescending = filters.SortOrder?.Trim().ToLower() == "desc";

                query = sortBy switch
                {
                    "title" => isDescending ? query.OrderByDescending(b => b.title) : query.OrderBy(b => b.title),
                    "author" => isDescending ? query.OrderByDescending(b => b.author) : query.OrderBy(b => b.author),
                    "year" => isDescending ? query.OrderByDescending(b => b.year) : query.OrderBy(b => b.year),
                    "average_rating" => isDescending ? query.OrderByDescending(b => b.average_rating) : query.OrderBy(b => b.average_rating),
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
            existingBook.ratings = updatedBookData.ratings;

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
        public async Task<IReadOnlyList<BookEntity>> GetRecommendedBooksAsync(Guid excludeBookId, int count, CancellationToken ct = default)
        {
            // Logic vẫn giữ nguyên: Lấy ngẫu nhiên
            return await _db.Books.AsNoTracking()
                .Where(b => b.BookId != excludeBookId) 
                .OrderBy(x => Guid.NewGuid())          
                .Take(count)                           
                .ToListAsync(ct);
        }
    }
}