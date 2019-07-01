namespace Willow.MessageBus.MassTransit
{
    using System;

    public class ConsumingBusBuilderException : Exception
    {
        public ConsumingBusBuilderException(string message)
            : base(message)
        { }

        public ConsumingBusBuilderException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}