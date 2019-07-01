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

        Task IConsumer<CreateEvent>.Consume(ConsumeContext<CreateEvent> context)
        {
            var command = context.Message;
            
            return Task.FromResult(0);

//            await repository.BeginSessionFor(command.EventId, state => aggregate.CreateEvent(state,
//                command.EventId,
//                command.Title,
//                command.Description));
        }
    }
}