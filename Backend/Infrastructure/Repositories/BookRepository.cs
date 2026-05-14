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
            
            if (filters.MinRating.HasValue || filters.MaxRating.HasValue)
            {
                // Ép kiểu sang double trước khi đưa vào truy vấn LINQ
                double minRating = (double)(filters.MinRating ?? 0);
                double maxRating = (double)(filters.MaxRating ?? 5);

                query = query.Where(b => 
                    (b.ratings_1 + b.ratings_2 + b.ratings_3 + b.ratings_4 + b.ratings_5) > 0 &&
                    ((double)(b.ratings_1 * 1 + b.ratings_2 * 2 + b.ratings_3 * 3 + b.ratings_4 * 4 + b.ratings_5 * 5) / 
                    (b.ratings_1 + b.ratings_2 + b.ratings_3 + b.ratings_4 + b.ratings_5)) >= minRating &&
                    ((double)(b.ratings_1 * 1 + b.ratings_2 * 2 + b.ratings_3 * 3 + b.ratings_4 * 4 + b.ratings_5 * 5) / 
                    (b.ratings_1 + b.ratings_2 + b.ratings_3 + b.ratings_4 + b.ratings_5)) <= maxRating
                );
            }
            if (filters.MinYear.HasValue)
            {
                query = query.Where(b => b.original_publication_year >= filters.MinYear.Value);
            }
            if (filters.MaxYear.HasValue)
            {
                query = query.Where(b => b.original_publication_year <= filters.MaxYear.Value);
            }
            
            query = query.OrderBy(b => b.book_id); 

            if (!string.IsNullOrWhiteSpace(filters.SortBy))
            {
                string sortBy = filters.SortBy.Trim().ToLower();
                bool isDescending = filters.SortOrder?.Trim().ToLower() == "desc";

                query = sortBy switch
                {
                    "title" => isDescending ? query.OrderByDescending(b => b.original_title) : query.OrderBy(b => b.original_title),
                    "author" => isDescending ? query.OrderByDescending(b => b.authors) : query.OrderBy(b => b.authors),
                    "year" => isDescending ? query.OrderByDescending(b => b.original_publication_year) : query.OrderBy(b => b.original_publication_year),
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

        public async Task<IReadOnlyList<BookEntity>> GetRecommendedBooksAsync(Guid excludeBookId, int count, CancellationToken ct = default)
        {
            return await _db.Books.AsNoTracking()
                .Where(b => b.book_id != excludeBookId) 
                .OrderBy(x => Guid.NewGuid())          
                .Take(count)                           
                .ToListAsync(ct);
        }
    }
}