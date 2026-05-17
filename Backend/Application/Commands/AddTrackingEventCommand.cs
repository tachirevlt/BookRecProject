using MediatR;
using System;
using System.Threading;
using Core.Entities;
using Core.Interfaces;
using Core.Entities;

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
            if (request.event_type != "view" && request.event_type != "purchase" && request.event_type != "add_favorite")
            {
                throw new ArgumentException("Invalid event_type. Must be 'product_click' or 'purchase' or 'add_favorite'.");
            }

            var trackingEvent = new TrackingEventEntity
            {
                user_id = request.user_id,
                event_type = request.event_type,
                book_id = request.book_id,
                created_at = DateTime.UtcNow
            };

            await _trackingRepository.AddTrackingEventAsync(trackingEvent);

            return true;
        }
    }
}
