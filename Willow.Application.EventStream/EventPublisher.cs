namespace Willow.Application.EventStream
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using BullOak.Repositories;
    using BullOak.Repositories.EventPublisher;
    using MassTransit;

    internal class EventPublisher : IPublishEvents
    {
        private readonly IPublishEndpoint publishEndpoint;

        public EventPublisher(IPublishEndpoint publishEndpoint)
        {
            this.publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        }

        Task IPublishEvents.Publish(ItemWithType @event, CancellationToken cancellationToken)
        {
            return publishEndpoint.Publish(@event.instance, @event.type, cancellationToken);
        }

        void IPublishEvents.PublishSync(ItemWithType @event)
        {
            publishEndpoint.Publish(@event.instance, @event.type).Wait();
        }
    }
}