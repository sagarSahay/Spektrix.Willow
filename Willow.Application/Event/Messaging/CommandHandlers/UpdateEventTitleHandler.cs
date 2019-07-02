using System;
using System.Threading.Tasks;
using BullOak.Repositories.Repository;
using MassTransit;
using Willow.Commands;

namespace Willow.Application.Event.Messaging.CommandHandlers
{
    internal class UpdateEventTitleHandler : IConsumer<UpdateEventTitle>
    {
        private readonly IStartSessions<Guid, IHoldEventState> repository;
        private readonly IEventAggregate aggregate;

        public UpdateEventTitleHandler(IStartSessions<Guid, IHoldEventState> repository, 
            IEventAggregate aggregate)
        {
            this.repository = repository;
            this.aggregate = aggregate;
        }

        async Task IConsumer<UpdateEventTitle>.Consume(ConsumeContext<UpdateEventTitle> context)
        {
            var command = context.Message;

            using (var session = await  repository.BeginSessionFor(command.EventId, false))
            {
                if (session == null)
                {
                    throw new Exception();
                }

                if (session.IsNewState)
                {
                    throw new Exception();
                }
                var state = session.GetCurrentState();

                var events = aggregate.ChangeEventTitle(state, command.EventId, command.Title);

                session.AddEvents(events);

                await session.SaveChanges();
            }
        }
    }
}
