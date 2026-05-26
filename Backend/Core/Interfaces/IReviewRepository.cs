using Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IReviewRepository
    {
        Task<ReviewEntity> AddReviewAsync(ReviewEntity review);
        Task<IEnumerable<ReviewEntity>> GetReviewsByBookIdAsync(Guid bookId, int pageNumber = 1, int pageSize = 3);
        Task<ReviewEntity?> GetReviewByIdAsync(Guid reviewId);
        Task<bool> DeleteReviewByIdAsync(Guid reviewId);
    }
}