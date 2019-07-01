namespace Willow.Application.Event
{
    using System;
    using System.Collections.Generic;
    using Messages.Common;

    public interface IEventAggregate
    {
        IEnumerable<Event> CreateEvent(IHoldEventState state, Guid id, string title, string description);
    }
}