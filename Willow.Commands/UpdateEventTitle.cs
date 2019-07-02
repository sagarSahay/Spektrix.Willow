using System;
using Willow.Messages.Common;

namespace Willow.Commands
{
    public class UpdateEventTitle : Command
    {
        public Guid EventId { get; set; }
        public string Title { get; set; }
    }
}