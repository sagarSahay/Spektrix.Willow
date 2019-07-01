namespace Willow.Application.Event
{
    using System;
    using System.Collections.Generic;
    using Events.v1;
    using Messages.Common;

    internal partial class EventAggregate : IEventAggregate
    {
        public IEnumerable<Event> CreateEvent(IHoldEventState state, Guid id, string title, string description)
        {
            if (string.IsNullOrEmpty(title))
            {
                throw new ArgumentNullException(nameof(title));
            }

            yield return new EventCreated(id, title, description);
        }
    }
}