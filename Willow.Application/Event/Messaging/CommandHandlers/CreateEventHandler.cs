namespace Willow.Application.Event.Messaging.CommandHandlers
{
    using System.Threading.Tasks;
    using Commands;
    using MassTransit;

    internal class CreateEventHandler : IConsumer<CreateEvent>
    {
        Task IConsumer<CreateEvent>.Consume(ConsumeContext<CreateEvent> context)
        {
            var command = context.Message;
            
            return Task.FromResult(0);
        }
    }
}