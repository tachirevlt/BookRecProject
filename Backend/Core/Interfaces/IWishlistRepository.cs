using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Entities;

namespace Core.Interfaces
{
    public interface IWishlistRepository
    {
        Task<UserWishlistEntity?> GetWishlistItemAsync(Guid userId, Guid bookId, CancellationToken ct = default);
        Task<IReadOnlyList<UserWishlistEntity>> GetUserWishlistAsync(Guid userId, CancellationToken ct = default);
        Task<UserWishlistEntity> AddToWishlistAsync(UserWishlistEntity item, CancellationToken ct = default);
        Task<bool> RemoveFromWishlistAsync(Guid userId, Guid bookId, CancellationToken ct = default);
        Task<bool> IsInWishlistAsync(Guid userId, Guid bookId, CancellationToken ct = default);
    }
}
