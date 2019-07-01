namespace Willow.Application.Event
{
    using System;
    using System.Collections;
    using Messages.Common;

    public interface IEventAggregate
    {
        IEnumerable<IEvent> CreateEvent(IHoldEventState state, Guid id, string title, string description);
    }
}