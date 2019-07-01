namespace Willow.Application.Event.EventAppliers
{
    using BullOak.Repositories.Appliers;
    using Events.v1;

    public class EventApplier: IApplyEvent<IHoldEventState, EventCreated>
    {
        public IHoldEventState Apply(IHoldEventState state, EventCreated @event)
        {
            state.Id = @event.EventId;
            state.Title = @event.Title;
            state.Description = @event.Description;

            return state;
        }
    }
}