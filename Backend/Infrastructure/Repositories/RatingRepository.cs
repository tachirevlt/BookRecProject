using Core.Entities;
using Core.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class RatingRepository : IRatingRepository
    {
        private readonly AppDbContext _context;

        public RatingRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<RatingEntity?> GetRatingAsync(Guid userId, Guid bookId, CancellationToken ct = default)
        {
            return await _context.Ratings.FirstOrDefaultAsync(r => r.user_id == userId && r.book_id == bookId, ct);
        }

        public async Task<RatingEntity> AddRatingAsync(RatingEntity rating, CancellationToken ct = default)
        {
            await _context.Ratings.AddAsync(rating, ct);
            await _context.SaveChangesAsync(ct);
            return rating;
        }

        public async Task<RatingEntity> UpdateRatingAsync(RatingEntity rating, CancellationToken ct = default)
        {
            _context.Ratings.Update(rating);
            await _context.SaveChangesAsync(ct);
            return rating;
        }

        public async Task<bool> DeleteRatingAsync(Guid userId, Guid bookId, CancellationToken ct = default)
        {
            var rating = await GetRatingAsync(userId, bookId, ct);
            if (rating == null) return false;

            _context.Ratings.Remove(rating);
            await _context.SaveChangesAsync(ct);
            return true;
        }
    }
}