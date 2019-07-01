namespace Willow.Denormalizer.Event
{
    using System.Threading.Tasks;
    using Common;
    using Events.v1;
    using MassTransit;
    using Repositories.Event.ViewModels;

    internal class EventDenormalizer :
        IConsumer<EventCreated>
    {
        private IDenormalizerRepository<EventVM> repository;

        public EventDenormalizer(IDenormalizerRepository<EventVM> repository)
        {
            this.repository = repository;
        }

        public async Task Consume(ConsumeContext<EventCreated> context)
        {
            var message = context.Message;
            var id = message.EventId.ToString();

            var eventVm = new EventVM()
            {
                EventId = message.EventId,
                Title = message.Title,
                Description = message.Description
            };

            var eventDoc = new DocumentBase<EventVM>() {VM = eventVm};

            await repository.Upsert(id, eventDoc);
        }
    }
}