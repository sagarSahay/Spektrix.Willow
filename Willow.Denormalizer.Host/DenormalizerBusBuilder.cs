namespace Willow.Denormalizer.Host
{
    using System;
    using Autofac;
    using Common;
    using MassTransit;
    using MessageBus.MassTransit;
    using MongoDB;

    public class DenormalizerBusBuilder
    {
        private readonly ContainerBuilder builder;

        private readonly Func<string, string> settingsResolver;

        public DenormalizerBusBuilder(Func<string, string> settingsResolver)
        {
            this.settingsResolver = settingsResolver;
            this.builder = ConfigureContainerBuilder();
        }
        
        public IContainer NewContainer()
        {
            return builder.Build();
        }
        
        private ContainerBuilder ConfigureContainerBuilder()
        {

            var containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterModule<Repositories.Module>();

            RegisterMassTransit(containerBuilder);
            RegisterDocumentStore(containerBuilder);

            return containerBuilder;
        }

        private void RegisterDocumentStore(ContainerBuilder builder)
        {
            var serverAddress = settingsResolver("MongoDb.ServerAddress");
            var serverPort = int.Parse(settingsResolver("MongoDb.ServerPort"));
            var databaseName = settingsResolver("MongoDb.DatabaseName");
            var userName = settingsResolver("MongoDb.UserName");
            var userPassword = settingsResolver("MongoDb.UserPassword");
            if (userPassword == "empty") userPassword = string.Empty;

            var settings = new MongoDbConnectionSettings(serverAddress, serverPort, databaseName, userName, userPassword);

            builder.Register(c => new MongoDb(settings))
                .As<IDocumentStore>();
        }

        private void RegisterMassTransit(ContainerBuilder containerBuilder)
        {
            var host = settingsResolver("RabbitMqHost");
            var username = settingsResolver("RabbitMqUser");
            var password = settingsResolver("RabbitMqPassword");
            var receiveQueue = settingsResolver("DenormalizerReceiveQueue");

            var denormalizerAssembly = typeof(Denormalizer.Module).Assembly;
            // Register consumer types in the container
            containerBuilder.RegisterModule(new ConsumersModule { ConsumersAssembly = denormalizerAssembly });
            // Resister bus with receiving endpoints per consumer
            containerBuilder.Register(c =>
                {
                    var bus = new ConsumingBusBuilder()
                        .WithRabbitMqHost(host, username, password)
                        .WithQueueNamePrefix($"{receiveQueue}.")
                        .WithConsumersFrom(denormalizerAssembly, t => c.Resolve(t))
                        .WithQueueNameFactory(t => { return t.FullName.Replace("Willow.Denormalizer.", string.Empty); })
                        .Build();

                    return bus;
                })
                .As<IBusControl>()
                .As<IBus>()
                .SingleInstance();
        }
    }
}