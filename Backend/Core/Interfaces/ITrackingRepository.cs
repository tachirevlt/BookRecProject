using System;
using System.Threading.Tasks;
using Core.Entities;

namespace Core.Interfaces
{
    public interface ITrackingRepository
    {
        Task AddTrackingEventAsync(TrackingEventEntity trackingEvent);
        Task IncrementBookStatsAsync(Guid bookId, string eventType);
    }
}
