namespace Willow.Events.v1
{
    using System;
    using Messages.Common;

    public class EventCreated : Event
    {
        public Guid EventId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public EventCreated(Guid eventId, string title, string description)
        {
            EventId = eventId;
            Title = title;
            Description = description;
        }
    }
}