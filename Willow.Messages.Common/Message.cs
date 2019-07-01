namespace Willow.Messages.Common
{
    using System;

    public class Message : IMessage
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
    }
}