namespace Willow.Application.Event.Messaging.CommandHandlers
{
    using System;
    using System.Threading.Tasks;
    using BullOak.Repositories.Repository;
    using Commands;
    using MassTransit;

    internal class CreateEventHandler : IConsumer<CreateEvent>
    {
        private readonly IStartSessions<Guid, IHoldEventState> repository;
        private readonly IEventAggregate aggregate;

        public CreateEventHandler(IStartSessions<Guid, IHoldEventState> repository, IEventAggregate aggregate)
        {
            this.repository = repository;
            this.aggregate = aggregate;
        }

        async Task IConsumer<CreateEvent>.Consume(ConsumeContext<CreateEvent> context)
        {
            var command = context.Message;
            
            using (var session =  await repository.BeginSessionFor(command.EventId, false))
            {
                if (session == null)
                {
                    // throw exception
                }

                if (!session.IsNewState)
                {
                    // throw concurrency exception
                }
                var state = session.GetCurrentState();

                var events = aggregate.CreateEvent(state, command.EventId, command.Title, command.Description);
                
                session.AddEvents(events);

                await session.SaveChanges();
            }
        }
    }
}