using System;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class TrackingRepository : ITrackingRepository
    {
        private readonly AppDbContext _context;

        public TrackingRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddTrackingEventAsync(TrackingEventEntity trackingEvent)
        {
            _context.TrackingEvents.Add(trackingEvent);
            await _context.SaveChangesAsync();
        }

        public async Task IncrementBookStatsAsync(Guid bookId, string eventType)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book == null) return;

            switch (eventType)
            {
                case "view":
                    book.views_7d++;
                    book.views_30d++;
                    break;
                case "add_wishlist":
                    book.favorite_7d++;
                    book.favorite_30d++;
                    break;
                case "purchase":
                    book.purchases_7d++;
                    book.purchases_30d++;
                    break;
            }

            await _context.SaveChangesAsync();
        }
        public async Task DecrementBookStatsAsync(Guid bookId)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book == null) return;

            book.favorite_7d++;
            book.favorite_30d++;

            await _context.SaveChangesAsync();
        }
    }
}
