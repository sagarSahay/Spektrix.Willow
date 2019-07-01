namespace Willow.Application.Host
{
    using System;
    using Autofac;
    using MassTransit;

    public class HostStarter : IDisposable
    {
        private readonly IContainer container;
        private readonly IBusControl busControl;
        
        private const string BusInstance = "Application";
        
        private readonly Func<string, string> settingsResolver;

        public HostStarter(
            Func<IContainer> containerBuilder,
            Func<string, string> settingsResolver)
        {
            if (containerBuilder == null) throw new ArgumentNullException(nameof(containerBuilder));
            this.settingsResolver = settingsResolver ?? throw new ArgumentNullException(nameof(settingsResolver));
            container = containerBuilder();
            
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