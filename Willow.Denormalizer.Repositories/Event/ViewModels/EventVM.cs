namespace Willow.Denormalizer.Repositories.Event.ViewModels
{
    using System;

    public class EventVM
    {
        public Guid EventId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}