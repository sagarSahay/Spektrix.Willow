namespace Willow.Denormalizer
{
    using Autofac;
    using Event;

    public class Module : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //builder.RegisterType<EventDenormalizer>().AsImplementedInterfaces();
            
            base.Load(builder);
        }
    }
}