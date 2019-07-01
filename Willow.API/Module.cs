namespace Willow.API
{
    using Autofac;
    using Events;

    public class Module : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<EventController>();

            builder.Build();
        }
    }
}