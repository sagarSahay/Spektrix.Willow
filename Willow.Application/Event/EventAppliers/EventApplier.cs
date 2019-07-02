namespace Willow.Application.Event.EventAppliers
{
    using BullOak.Repositories.Appliers;
    using Events.v1;

    public class EventApplier: IApplyEvent<IHoldEventState, EventCreated>, 
        IApplyEvent<IHoldEventState, EventTitleUpdated>
    {
        public IHoldEventState Apply(IHoldEventState state, EventCreated @event)
        {
            state.Id = @event.EventId;
            state.Title = @event.Title;
            state.Description = @event.Description;

            return state;
        }

        public IHoldEventState Apply(IHoldEventState state, EventTitleUpdated @event)
        {
            state.Title = @event.Title;
            return state;
        }
    }
}