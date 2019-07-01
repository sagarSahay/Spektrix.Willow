namespace Willow.Denormalizer.Repositories.Event
{
    using Common;
    using ViewModels;

    internal class EventRepository : CommonVMRepo<EventVM>, 
        IEventQueryRepository
    {
        public EventRepository(IDocumentStore db)
            : base(db, typeof(EventVM).FullName)
        {
        }
    }
}