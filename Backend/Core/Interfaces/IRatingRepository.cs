using Core.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IRatingRepository
    {
        Task<RatingEntity?> GetRatingAsync(Guid userId, Guid bookId, CancellationToken ct = default);
        Task<RatingEntity> AddRatingAsync(RatingEntity rating, CancellationToken ct = default);
        Task<RatingEntity> UpdateRatingAsync(RatingEntity rating, CancellationToken ct = default);
        Task<bool> DeleteRatingAsync(Guid userId, Guid bookId, CancellationToken ct = default);
    }
}