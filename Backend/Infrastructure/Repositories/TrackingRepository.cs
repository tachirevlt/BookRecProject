using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Persistence;

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
    }
}
