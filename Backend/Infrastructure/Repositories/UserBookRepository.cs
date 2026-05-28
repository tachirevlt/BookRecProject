using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class UserBookRepository : IUserBookRepository
    {
        private readonly AppDbContext _db;

        public UserBookRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<UserBookEntity?> GetUserBookAsync(Guid userId, Guid bookId, CancellationToken ct = default)
        {
            return await _db.UserBooks
                .Include(ub => ub.Book)
                .FirstOrDefaultAsync(ub => ub.user_id == userId && ub.book_id == bookId, ct);
        }

        public async Task<IReadOnlyList<UserBookEntity>> GetUserBooksAsync(Guid userId, CancellationToken ct = default)
        {
            return await _db.UserBooks
                .AsNoTracking()
                .Include(ub => ub.Book)
                .Where(ub => ub.user_id == userId)
                .OrderByDescending(ub => ub.purchase_date)
                .ToListAsync(ct);
        }

        public async Task<UserBookEntity> AddUserBookAsync(UserBookEntity userBook, CancellationToken ct = default)
        {
            await _db.UserBooks.AddAsync(userBook, ct);
            await _db.SaveChangesAsync(ct);
            return userBook;
        }

        public async Task<bool> HasPurchasedAsync(Guid userId, Guid bookId, CancellationToken ct = default)
        {
            return await _db.UserBooks
                .AnyAsync(ub => ub.user_id == userId && ub.book_id == bookId, ct);
        }

        public async Task<UserBookEntity?> UpdateReadingProgressAsync(Guid userId, Guid bookId, int currentChapter, CancellationToken ct = default)
        {
            var userBook = await _db.UserBooks
                .FirstOrDefaultAsync(ub => ub.user_id == userId && ub.book_id == bookId, ct);
            if (userBook == null) return null;

            userBook.current_chapter = currentChapter;
            userBook.last_read_at = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
            return userBook;
        }
        public async Task<List<Guid>> GetUsersWhoBoughtBookAsync(Guid bookId, CancellationToken ct = default)
        {
            return await _db.UserBooks
                .Where(ub => ub.book_id == bookId)
                .Select(ub => ub.user_id)
                .ToListAsync(ct);
        }
        public async Task<IEnumerable<Guid>> CheckUsersWhoBoughtBookAsync(
            Guid bookId, 
            IEnumerable<Guid> userIds, 
            CancellationToken cancellationToken)
        {
            // Lệnh này sẽ sinh ra SQL: SELECT user_id FROM UserBooks WHERE book_id = @bookId AND user_id IN (@id1, @id2,...)
            return await _db.UserBooks
                .Where(ub => ub.book_id == bookId && userIds.Contains(ub.user_id))
                .Select(ub => ub.user_id)
                .ToListAsync(cancellationToken);
        }
    }
}
