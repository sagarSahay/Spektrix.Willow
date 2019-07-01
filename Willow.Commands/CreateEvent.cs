namespace Willow.Commands
{
    using System;
    using Messages.Common;

    public class CreateEvent : Command
    {
        public string Title { get; set; }
        public Guid EventId { get; set; }
        public string Description { get; set; }
    }
}