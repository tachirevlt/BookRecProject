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
    public class WishlistRepository : IWishlistRepository
    {
        private readonly AppDbContext _db;

        public WishlistRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<UserWishlistEntity?> GetWishlistItemAsync(Guid userId, Guid bookId, CancellationToken ct = default)
        {
            return await _db.Wishlists
                .Include(w => w.Book)
                .FirstOrDefaultAsync(w => w.user_id == userId && w.book_id == bookId, ct);
        }

        public async Task<IReadOnlyList<UserWishlistEntity>> GetUserWishlistAsync(Guid userId, CancellationToken ct = default)
        {
            return await _db.Wishlists
                .AsNoTracking()
                .Include(w => w.Book)
                .Where(w => w.user_id == userId)
                .OrderByDescending(w => w.added_at)
                .ToListAsync(ct);
        }

        public async Task<UserWishlistEntity> AddToWishlistAsync(UserWishlistEntity item, CancellationToken ct = default)
        {
            await _db.Wishlists.AddAsync(item, ct);
            await _db.SaveChangesAsync(ct);
            return item;
        }

        public async Task<bool> RemoveFromWishlistAsync(Guid userId, Guid bookId, CancellationToken ct = default)
        {
            var item = await _db.Wishlists
                .FirstOrDefaultAsync(w => w.user_id == userId && w.book_id == bookId, ct);
            if (item == null) return false;
            _db.Wishlists.Remove(item);
            await _db.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> IsInWishlistAsync(Guid userId, Guid bookId, CancellationToken ct = default)
        {
            return await _db.Wishlists
                .AnyAsync(w => w.user_id == userId && w.book_id == bookId, ct);
        }
    }
}
