using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Entities;

namespace Core.Interfaces
{
    public interface IUserBookRepository
    {
        Task<UserBookEntity?> GetUserBookAsync(Guid userId, Guid bookId, CancellationToken ct = default);
        Task<IReadOnlyList<UserBookEntity>> GetUserBooksAsync(Guid userId, CancellationToken ct = default);
        Task<UserBookEntity> AddUserBookAsync(UserBookEntity userBook, CancellationToken ct = default);
        Task<bool> HasPurchasedAsync(Guid userId, Guid bookId, CancellationToken ct = default);
        Task<UserBookEntity?> UpdateReadingProgressAsync(Guid userId, Guid bookId, int currentChapter, CancellationToken ct = default);
    }
}
