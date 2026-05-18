using MediatR;
using System;
using System.Threading;
using Core.Entities;
using Core.Interfaces;

namespace Application.Commands
{
    public class AddTrackingEventCommand : IRequest<bool>
    {
        public Guid user_id { get; set; }
        public string event_type { get; set; } = null!;
        public Guid book_id { get; set; }
    }

    public class AddTrackingEventCommandHandler : IRequestHandler<AddTrackingEventCommand, bool>
    {
        private readonly ITrackingRepository _trackingRepository;

        public AddTrackingEventCommandHandler(ITrackingRepository trackingRepository)
        {
            _trackingRepository = trackingRepository;
        }

        public async Task<bool> Handle(AddTrackingEventCommand request, CancellationToken cancellationToken)
        {
            // Tracking chỉ nhận "view" — favorite/purchase được xử lý qua API riêng
            if (request.event_type != "view")
            {
                throw new ArgumentException("Invalid event_type. Tracking chỉ chấp nhận 'view'.");
            }

            var trackingEvent = new TrackingEventEntity
            {
                user_id = request.user_id,
                event_type = request.event_type,
                book_id = request.book_id,
                created_at = DateTime.UtcNow
            };

            await _trackingRepository.AddTrackingEventAsync(trackingEvent);

            // Tăng bộ đếm views_7d và views_30d của sách
            await _trackingRepository.IncrementBookStatsAsync(request.book_id, "view");

            return true;
        }
    }
}
