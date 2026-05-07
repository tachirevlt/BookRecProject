using Core.Entities;
using Core.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
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

        public async Task<ReviewEntity> AddReviewAsync(ReviewEntity review)
        {
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task<IEnumerable<ReviewEntity>> GetReviewsByBookIdAsync(Guid bookId)
        {
            return await _context.Reviews
                .Where(r => r.book_id == bookId)
                .OrderByDescending(r => r.time)
                .ToListAsync();
        }

        public async Task<bool> DeleteReviewByBookIdAsync(Guid bookId, Guid userId)
        {
            var review = await _context.Reviews
                .FirstOrDefaultAsync(r => r.book_id == bookId && r.user_id == userId);

            if (review == null) return false; 

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<ReviewEntity?> GetReviewByIdAsync(Guid reviewId)
        {
            return await _context.Reviews.FindAsync(reviewId);
        }

        public async Task<bool> DeleteReviewByIdAsync(Guid reviewId)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review == null) return false; 

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}