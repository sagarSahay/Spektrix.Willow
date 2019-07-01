namespace Willow.MessageBus.MassTransit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using global::MassTransit;
    
    using ConsumersCollectionType = System.Collections.Generic.IDictionary<System.Type, System.Func<System.Type, object>>;
    using QueueNameFactoryType = System.Func<System.Type, string>;
    using BusFactoryType = System.Func<System.Action<global::MassTransit.RabbitMqTransport.IRabbitMqBusFactoryConfigurator>, global::MassTransit.IBusControl>;
    using BusFactoryConfiguratorType = System.Action<global::MassTransit.RabbitMqTransport.IRabbitMqBusFactoryConfigurator>;
    using ReceiveEndpointConfiguratorType = System.Action<global::MassTransit.RabbitMqTransport.IRabbitMqReceiveEndpointConfigurator>;


    public class ConsumingBusBuilder
    {
        private string hostUri;
        private string username;
        private string password;
        private string queueNamePrefix;
        private QueueNameFactoryType queueNameFactory;
        private BusFactoryType busFactory;
        private BusFactoryConfiguratorType busFactoryCustomizations;
        private ReceiveEndpointConfiguratorType receiveEndpointCustomizations;
        private ConsumersCollectionType consumers;

        public ConsumingBusBuilder()
        {
            queueNameFactory = DefaultQueueNameFactory;
            busFactory = DefaultBusControlFactory;
        }

        public ConsumingBusBuilder WithRabbitMqHost(string hostUri, string username, string password)
        {
            if (string.IsNullOrEmpty(hostUri)) throw new ArgumentNullException(nameof(hostUri));
            if (string.IsNullOrEmpty(username)) throw new ArgumentNullException(nameof(username));

            this.hostUri = hostUri;
            this.username = username;
            this.password = password;

            return this;
        }

        public ConsumingBusBuilder WithConsumers(ConsumersCollectionType consumers)
        {
            if (consumers == null) throw new ArgumentNullException(nameof(consumers));
            if (consumers.Any(kvp => !kvp.Key.ImplementsConsumerInterface())) throw new ArgumentException($"One of {nameof(consumers)} does not implement IConsumer<>");
            if (consumers.Any(kvp => kvp.Value == null)) throw new ArgumentException($"One of {nameof(consumers)} does not provide instance factory");
            this.consumers = consumers;
            return this;
        }

        public ConsumingBusBuilder WithConsumersFrom(Assembly assembly, Func<Type, object> instanceFactory)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));
            if (instanceFactory == null) throw new ArgumentNullException(nameof(instanceFactory));
            return WithConsumers(GetConsumersFromAssembly(assembly, instanceFactory));
        }

        public ConsumingBusBuilder AndConsumersFrom(Assembly assembly, Func<Type, object> instanceFactory)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));
            if (instanceFactory == null) throw new ArgumentNullException(nameof(instanceFactory));
            if (consumers == null) throw new ConsumingBusBuilderException("No consumers to append to");

            var consumersToAppend = GetConsumersFromAssembly(assembly, instanceFactory);

            var mergedConsumers = (new[] { consumers, consumersToAppend })
                .SelectMany(d => d)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            return WithConsumers(mergedConsumers);
        }

        public ConsumingBusBuilder WithQueueNameFactory(QueueNameFactoryType queueNameFactory)
        {
            this.queueNameFactory = queueNameFactory ?? throw new ArgumentNullException(nameof(queueNameFactory));
            return this;
        }

        public ConsumingBusBuilder WithQueueNamePrefix(string queueNamePrefix)
        {
            if (string.IsNullOrEmpty(queueNamePrefix)) throw new ArgumentNullException(nameof(queueNamePrefix));
            this.queueNamePrefix = queueNamePrefix;
            return this;
        }

        public ConsumingBusBuilder WithBusFactrory(BusFactoryType busFactory)
        {
            this.busFactory = busFactory ?? throw new ArgumentNullException(nameof(busFactory));
            return this;
        }

        public ConsumingBusBuilder WithBusFactoryCustomizations(BusFactoryConfiguratorType busFactoryCustomizations)
        {
            this.busFactoryCustomizations = busFactoryCustomizations ?? throw new ArgumentNullException(nameof(busFactoryCustomizations));
            return this;
        }

        public ConsumingBusBuilder WithReceiveEndpointCustomizations(ReceiveEndpointConfiguratorType receiveEndpointCustomizations)
        {
            this.receiveEndpointCustomizations = receiveEndpointCustomizations ?? throw new ArgumentNullException(nameof(receiveEndpointCustomizations));
            return this;
        }

        public IBusControl Build()
        {
            if (string.IsNullOrEmpty(hostUri)) throw new ConsumingBusBuilderException($"{nameof(hostUri)} is not specified");
            if (string.IsNullOrEmpty(username)) throw new ConsumingBusBuilderException($"{nameof(username)} is not specified");
            if (string.IsNullOrEmpty(queueNamePrefix)) throw new ConsumingBusBuilderException($"{nameof(queueNamePrefix)} is not specified");
            if (queueNameFactory == null) throw new ConsumingBusBuilderException($"{nameof(queueNameFactory)} is not specified");
            if (consumers == null || consumers.Count() == 0) throw new ConsumingBusBuilderException($"{nameof(consumers)} are not provided");

            var bus = busFactory(bfc =>
            {
                busFactoryCustomizations?.Invoke(bfc);

                var host = bfc.Host(new Uri(hostUri), h =>
                {
                    h.Username(username);
                    h.Password(password);
                });

                foreach (var kvp in consumers)
                {
                    var consumerType = kvp.Key;
                    var consumerFactory = kvp.Value;
                    var consumerInstance = consumerFactory(consumerType);
                    var queueName = string.Concat(queueNamePrefix, queueNameFactory(consumerType));

                    bfc.ReceiveEndpoint(host, queueName, rec =>
                    {
                        receiveEndpointCustomizations?.Invoke(rec);
                        rec.Consumer(consumerType, _ => consumerInstance);
                    });
                }
            });

            return bus;
        }

        private static IBusControl DefaultBusControlFactory(BusFactoryConfiguratorType configure)
        {
            return Bus.Factory.CreateUsingRabbitMq(configure);
        }

        private static string DefaultQueueNameFactory(Type consumerType)
        {
            return consumerType.FullName;
        }

        private static ConsumersCollectionType GetConsumersFromAssembly(Assembly assembly, Func<Type, object> instanceFactory)
        {
            var consumerTypes = assembly.GetTypes()
                .Where(t => t.ImplementsConsumerInterface() && t.AllowsAutomaticRegistration())
                .ToList();

            var consumersCollection = new Dictionary<Type, Func<Type, object>>();
            consumerTypes.ForEach(t =>
            {
                consumersCollection.Add(t, instanceFactory);
            });

            return consumersCollection;
        }
    }
}