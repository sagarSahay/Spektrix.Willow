using Willow.Commands;

namespace Willow.Application
{
    using Autofac;
    using Event;
    using Event.Messaging.CommandHandlers;

    public class Module : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CreateEventHandler>().AsImplementedInterfaces();

            builder.RegisterType<UpdateEventTitleHandler>().AsImplementedInterfaces();
            
            builder.RegisterType<EventAggregate>().AsImplementedInterfaces();
            
            base.Load(builder);
        }
    }
}