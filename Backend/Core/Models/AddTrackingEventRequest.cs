using System;

namespace Core.Models
{
    public class AddTrackingEventRequest
    {
        public string event_type { get; set; } = null!;
        public Guid book_id { get; set; }
    }
}
