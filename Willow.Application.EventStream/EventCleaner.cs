namespace Willow.Application.EventStream
{
    using System;
    using System.Threading.Tasks;
    using EventStore.ClientAPI;

    public class EventCleaner
    {
        private readonly IEventStoreConnection connection;

        public EventCleaner(IEventStoreConnection connection)
        {
            this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        // FIXME: Find equivalent of eventStore.Advanced.Purge()
        public Task ClearEventStore() => throw new NotImplementedException();
    }
}