namespace Willow.Application
{
    using Autofac;
    using Event.Messaging.CommandHandlers;

    public class Module : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CreateEventHandler>().AsImplementedInterfaces();
            
            base.Load(builder);
        }
    }
}