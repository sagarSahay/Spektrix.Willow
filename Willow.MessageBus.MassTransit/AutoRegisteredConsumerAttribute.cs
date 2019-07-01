namespace Willow.MessageBus.MassTransit
{
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class AutoRegisteredConsumerAttribute : Attribute
    {
        public bool AutoRegistered { get; private set; }

        public AutoRegisteredConsumerAttribute(bool autoRegistered = true)
        {
            AutoRegistered = autoRegistered;
        }
    }
}