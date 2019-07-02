using System;
using Willow.Messages.Common;

namespace Willow.Events.v1
{
    public class EventTitleUpdated : Event
    {
        public EventTitleUpdated(Guid eventId, string title)
        {
            EventId = eventId;
            Title = title;
        }

        public Guid EventId { get; set; }
        public string Title { get; set; }
    }
}