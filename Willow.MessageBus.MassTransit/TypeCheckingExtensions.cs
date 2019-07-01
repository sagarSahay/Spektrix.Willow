namespace Willow.MessageBus.MassTransit
{
    using System;
    using System.Linq;
    using global::MassTransit;

    internal static class TypeCheckingExtensions
    {
        public static bool ImplementsConsumerInterface(this Type t)
        {
            return t != null && t.IsClass && !t.IsAbstract
                   && t.GetInterfaces()
                       .Where(i => i.IsGenericType)
                       .Any(i => i.GetGenericTypeDefinition() == typeof(IConsumer<>));
        }

        public static bool AllowsAutomaticRegistration(this Type t)
        {
            if (t == null) return false;
            var attribute = Attribute.GetCustomAttribute(t, typeof(AutoRegisteredConsumerAttribute)) as AutoRegisteredConsumerAttribute;
            if (attribute == null) return true;
            return attribute.AutoRegistered;
        }
    }
}