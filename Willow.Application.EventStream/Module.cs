namespace Willow.Application.EventStream
{
    using System.Reflection;
    using Autofac;
    using BullOak.Repositories;
    using BullOak.Repositories.Config;
    using BullOak.Repositories.EventStore;
    using EventStore.ClientAPI;
    using MassTransit;

    public class Module : Autofac.Module
    {
        public string ConnectionString { private get; set; }
        public Assembly AppliersAssembly { private get; set; }

        protected override void Load(ContainerBuilder builder)
        {
            builder
                .Register(x =>
                {
                    var settings = ConnectionSettings
                        .Create()
                        .KeepReconnecting()
                        .KeepRetrying()
                        .FailOnNoServerResponse();
                    
                    var connection = EventStoreConnection.Create(ConnectionString, settings);
                    connection.ConnectAsync().Wait();
                    return connection;
                })
                .As<IEventStoreConnection>()
                .InstancePerLifetimeScope();

            builder.RegisterGeneric(typeof(EventStoreRepository<,>))
                .AsImplementedInterfaces();

            builder
                .Register(
                    x => BullOak.Repositories.Configuration
                        .Begin()
                        .WithDefaultCollection()
                        .WithDefaultStateFactory()
                        .AlwaysUseThreadSafe()
                        .WithEventPublisher(new EventPublisher(x.Resolve<IPublishEndpoint>()))
                        .WithAnyAppliersFrom(AppliersAssembly).AndNoMoreAppliers()
                        .WithNoUpconverters()
                        .Build())
                .As<IHoldAllConfiguration>();

            builder.RegisterType<EventCleaner>()
                .AsSelf();

            base.Load(builder);
        }
    }
}