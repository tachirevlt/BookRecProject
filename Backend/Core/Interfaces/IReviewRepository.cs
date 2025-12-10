using Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IReviewRepository
    {
        Task<ReviewEntity> AddReviewAsync(ReviewEntity review);
        Task<IEnumerable<ReviewEntity>> GetReviewsByBookIdAsync(Guid bookId);
        Task<double> GetAverageRatingAsync(Guid bookId);
        Task<bool> DeleteReviewByBookIdAsync(Guid bookId, Guid userId);
    }
}