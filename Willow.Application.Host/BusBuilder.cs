namespace Willow.Application.Host
{
    using System;
    using Autofac;
    using MassTransit;

    public class BusBuilder
    {
        private readonly Func<string, string> settingsResolver;
        private readonly ContainerBuilder builder;

        public BusBuilder(Func<string, string> settingsResolver, Action<ContainerBuilder> extraRegistrations = null)
        {
            this.settingsResolver = settingsResolver ?? throw new ArgumentNullException(nameof(settingsResolver));

            builder = ConfigureContainerBuilder();
            extraRegistrations?.Invoke(builder);
        }

        private ContainerBuilder ConfigureContainerBuilder()
        {
            var builder = new ContainerBuilder();

            var host = settingsResolver("RabbitMqHost");
            var receiveQueue = settingsResolver("ReceiveQueue");
            
            builder.RegisterModule(new EventStream.Module
            {
                AppliersAssembly = typeof(Application.Module).Assembly,
                ConnectionString = settingsResolver("Willow.Application.EventStore")
            });

            builder.RegisterModule(new Application.Module());
            RegisterMassTransit(builder);
            return builder;
        }
        
        public IContainer NewContainer()
        {
            return builder.Build();
        }

        private void RegisterMassTransit(ContainerBuilder builder)
        {
            var host = settingsResolver("RabbitMqHost");
            var username = settingsResolver("RabbitMqUser");
            var password = settingsResolver("RabbitMqPassword");
            var receiveQueue = settingsResolver("ReceiveQueue");

            builder.Register(c =>
                {
                    var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
                    {
                        var hostInstance = sbc.Host(new Uri(host), h =>
                        {
                            h.Username(username);
                            h.Password(password);
                        });

                        sbc.ReceiveEndpoint(hostInstance, receiveQueue, ce => { ce.LoadFrom(c); });
                    });

                    return bus;
                })
                .As<IBusControl>()
                .As<IBus>()
                .As<IPublishEndpoint>()
                .SingleInstance();
        }
    }
}