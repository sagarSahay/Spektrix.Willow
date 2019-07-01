namespace Willow.Denormalizer.Host
{
    using System;
    using Autofac;
    using MassTransit;

    public class DenormalizerHostStarter : IDisposable
    {
        private readonly string busInstance = "Denormalizer";
        private readonly IContainer container;
        private readonly IBusControl busControl;

        public DenormalizerHostStarter(Func<IContainer> containerBuilder)
        {
            container = (containerBuilder ?? throw new ArgumentNullException(nameof(containerBuilder)))();
            busControl = container.Resolve<IBusControl>();

            busControl.Start();
        }

        public void Dispose()
        {
            busControl?.Stop();

            container?.Dispose();
        }
    }
}