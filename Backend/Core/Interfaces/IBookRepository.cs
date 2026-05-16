using Core.Entities;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IBookRepository
    {
        Task<BookEntity?> GetBookByIdAsync(Guid id, CancellationToken ct = default);
        Task<(IReadOnlyList<BookEntity> Books, int TotalCount)> GetAllBooksWithPaginationAndFilteringAsync(
            PaginationParams pagination,
            BookFilterParams filters,
            CancellationToken cancellationToken);
        Task<BookEntity> AddBookAsync(BookEntity entity, CancellationToken ct = default);
        Task<BookEntity> UpdateBookAsync(Guid bookId, BookEntity entity, CancellationToken ct = default);
        Task<bool> DeleteBookAsync(Guid bookId, CancellationToken ct = default);
        Task<IReadOnlyList<BookEntity>> GetRecommendedBooksAsync(int count, CancellationToken ct = default);
    }
}