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
            // Xử lý trường hợp không tìm thấy nếu cần (ví dụ throw exception)
            if (book == null) throw new KeyNotFoundException($"Không tìm thấy sách với ID: {id}");
            return book;
        }

        public async Task<(IReadOnlyList<BookEntity> Books, int TotalCount)> GetAllBooksWithPaginationAndFilteringAsync(
                PaginationParams pagination,
                BookFilterParams filters,
                CancellationToken cancellationToken)
            {
                // 1. Khởi tạo truy vấn
                IQueryable<BookEntity> query = _db.Books.AsNoTracking();

                // 2. Xử lý Lọc (Filtering) - Đã bao gồm các tham số mới

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
                    query = query.Where(b => b.tag_name != null && b.tag_name.Contains(filters.Genre));
                }
                
                // LỌC PHẠM VI 1: Lọc theo Xếp hạng Trung bình (average_rating)
                if (filters.MinRating.HasValue)
                {
                    query = query.Where(b => b.average_rating >= filters.MinRating.Value);
                }

                if (filters.MaxRating.HasValue)
                {
                    query = query.Where(b => b.average_rating <= filters.MaxRating.Value);
                }
                
                // LỌC PHẠM VI 2: Lọc theo Năm Xuất bản (year)
                if (filters.MinYear.HasValue)
                {
                    query = query.Where(b => b.year >= filters.MinYear.Value);
                }
                
                if (filters.MaxYear.HasValue)
                {
                    query = query.Where(b => b.year <= filters.MaxYear.Value);
                }
                
                
                // Mặc định sắp xếp theo ID để đảm bảo tính ổn định (Required for Skip/Take)
                query = query.OrderBy(b => b.BookId); 

                // Nếu có tiêu chí sắp xếp, áp dụng sắp xếp động
                if (!string.IsNullOrWhiteSpace(filters.SortBy))
                {
                    // Chuẩn hóa tên thuộc tính sắp xếp
                    string sortBy = filters.SortBy.ToLowerInvariant();
                    
                    // Xác định thứ tự
                    bool isDescending = filters.SortOrder?.Equals("desc", StringComparison.OrdinalIgnoreCase) == true;

                    // Xử lý Sắp xếp theo các trường cụ thể
                    query = sortBy switch
                    {
                        "title" => isDescending ? query.OrderByDescending(b => b.title) : query.OrderBy(b => b.title),
                        "author" => isDescending ? query.OrderByDescending(b => b.author) : query.OrderBy(b => b.author),
                        "year" => isDescending ? query.OrderByDescending(b => b.year) : query.OrderBy(b => b.year),
                        "average_rating" => isDescending ? query.OrderByDescending(b => b.average_rating) : query.OrderBy(b => b.average_rating),
                        // Có thể thêm các trường khác tại đây, ví dụ: "ratings", "books_count", ...
                        _ => query // Giữ nguyên thứ tự nếu SortBy không hợp lệ
                    };
                }

                // 4. Đếm tổng số lượng (sau khi lọc, trước khi phân trang)
                int totalCount = await query.CountAsync(cancellationToken);

                // 5. Xử lý Phân trang (Pagination)
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
            return book; // Trả về entity đã được thêm (EF Core sẽ cập nhật ID)
        }

        public async Task<BookEntity> UpdateBookAsync(Guid bookId, BookEntity updatedBookData, CancellationToken ct = default)
        {
            var existingBook = await _db.Books.FindAsync(new object?[] { bookId }, ct);
            if (existingBook is null)
            {
                // Nên throw exception hoặc trả về null/Result pattern tùy thiết kế
                throw new KeyNotFoundException($"Không tìm thấy sách với ID: {bookId}");
            }

            // Cập nhật các thuộc tính cần thiết từ updatedBookData vào existingBook
            existingBook.title = updatedBookData.title;
            existingBook.author = updatedBookData.author;
            existingBook.tag_name = updatedBookData.tag_name;
            existingBook.year = updatedBookData.year;
            existingBook.books_count = updatedBookData.books_count;
            existingBook.work_id = updatedBookData.work_id;
            existingBook.isbn = updatedBookData.isbn;
            existingBook.language_code = updatedBookData.language_code;
            existingBook.average_rating = updatedBookData.average_rating;
            existingBook.ratings = updatedBookData.ratings;

            _db.Books.Update(existingBook); // Update entity đã được track
            await _db.SaveChangesAsync(ct);
            return existingBook; // Trả về entity đã được cập nhật
        }

        public async Task<bool> DeleteBookAsync(Guid id, CancellationToken ct = default)
        {
            var entity = await _db.Books.FindAsync(new object?[] { id }, ct);
            if (entity is null)
            {
                return false; // Không tìm thấy để xóa
            }
            _db.Books.Remove(entity);
            await _db.SaveChangesAsync(ct);
            return true; // Xóa thành công
        }
    }
}
