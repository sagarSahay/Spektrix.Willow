using System;
using System.Collections.Generic;
using System.Text;
using Willow.Events.v1;

namespace Willow.Application.Event
{
    internal partial class EventAggregate : IEventAggregate
    {
        public IEnumerable<Messages.Common.Event> ChangeEventTitle(IHoldEventState state, Guid id, string title)
        {
            if (string.IsNullOrEmpty(title))
            {
                throw new ArgumentNullException(nameof(title));
            }

            yield return new EventTitleUpdated(id, title);
        }
    }
}
