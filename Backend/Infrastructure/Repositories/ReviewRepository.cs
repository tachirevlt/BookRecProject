using Core.Entities;
using Core.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly AppDbContext _context;

        public ReviewRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<double> GetAverageRatingAsync(Guid bookId)
        {
             var ratings = await _context.Reviews.Where(r => r.BookId == bookId).Select(r => r.Rating).ToListAsync();
             if (!ratings.Any()) return 0;
             return ratings.Average();
        }
        public async Task<ReviewEntity> AddReviewAsync(ReviewEntity review)
        {
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task<IEnumerable<ReviewEntity>> GetReviewsByBookIdAsync(Guid bookId)
        {
            return await _context.Reviews
                .Where(r => r.BookId == bookId)
                .Include(r => r.User) // Giữ lại nếu muốn hiện tên người chấm điểm
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }


        public async Task<bool> DeleteReviewByBookIdAsync(Guid bookId, Guid userId)
        {
            // Tìm review khớp cả BookId và UserId
            var review = await _context.Reviews
                .FirstOrDefaultAsync(r => r.BookId == bookId && r.UserId == userId);

            if (review == null)
            {
                return false; 
            }

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}