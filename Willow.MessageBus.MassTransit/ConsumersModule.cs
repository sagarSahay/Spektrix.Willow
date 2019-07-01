namespace Willow.MessageBus.MassTransit
{
    using System.Linq;
    using System.Reflection;
    using Autofac;

    public class ConsumersModule : Autofac.Module
    {
        public Assembly ConsumersAssembly { get; set; }

        protected override void Load(ContainerBuilder builder)
        {
            ConsumersAssembly?.GetTypes()
                .Where(t => t.ImplementsConsumerInterface() && t.AllowsAutomaticRegistration())
                .ToList()
                .ForEach(t =>
                {
                    builder.RegisterType(t);
                });
        }
    }
}