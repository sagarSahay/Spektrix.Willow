namespace Willow.Denormalizer.Repositories
{
    using Autofac;
    using Common;
    using Event;

    public class Module : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c => new EventRepository(c.Resolve<IDocumentStore>()))
                .SingleInstance()
                .AsSelf()
                .AsImplementedInterfaces();
        }
    }
}