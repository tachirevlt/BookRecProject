using System;
using System.Linq;
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

        // Cấu hình giới hạn tracking cho mỗi user
        private const int MaxTrackingPerUser = 300;
        // Ngưỡng kích hoạt dọn dẹp (Để tránh phải gọi lệnh Delete liên tục mỗi lần insert)
        private const int CleanupThreshold = 320; 

        public TrackingRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddTrackingEventAsync(TrackingEventEntity trackingEvent)
        {
            // 1. Lưu tracking mới vào Database
            _context.TrackingEvents.Add(trackingEvent);
            await _context.SaveChangesAsync();

            // 2. Kiểm tra và dọn dẹp dữ liệu cũ của chính User này
            var currentCount = await _context.TrackingEvents
                .Where(t => t.user_id == trackingEvent.user_id)
                .CountAsync();

            if (currentCount > CleanupThreshold)
            {
                var itemsToRemoveCount = currentCount - MaxTrackingPerUser;

                // Lấy ra các tracking cũ nhất (sắp xếp tăng dần theo thời gian tạo)
                var oldEventsToDelete = await _context.TrackingEvents
                    .Where(t => t.user_id == trackingEvent.user_id)
                    .OrderBy(t => t.created_at) 
                    .Take(itemsToRemoveCount)
                    .ToListAsync();

                if (oldEventsToDelete.Any())
                {
                    _context.TrackingEvents.RemoveRange(oldEventsToDelete);
                    await _context.SaveChangesAsync();
                }
            }
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

            book.favorite_7d = Math.Max(0, book.favorite_7d - 1);
            book.favorite_30d = Math.Max(0, book.favorite_30d - 1);

            await _context.SaveChangesAsync();
        }
    }
}