namespace Willow.Application.Event
{
    using System;
    using System.Collections;
    using System.Diagnostics.Tracing;
    using Events.v1;

    internal partial class EventAggregate : IEventAggregate
    {
        public IEnumerable CreateEvent(IHoldEventState state, Guid id, string title, string description)
        {
            if (string.IsNullOrEmpty(title))
            {
                throw new ArgumentNullException(nameof(title));
            }

            yield return new EventCreated(id, title, description);
        }
    }
}