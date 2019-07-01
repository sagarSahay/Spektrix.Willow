namespace Willow.Application.Host.Console
{
    using System;

    public class ApplicationInProcessHost : IDisposable
    {
        private readonly IDisposable serviceHandle;

        public ApplicationInProcessHost(Func<string, string> settingsResolver)
        {
            if (settingsResolver == null) throw new ArgumentNullException(nameof(settingsResolver));

            var applicationBusBuilder = new BusBuilder(settingsResolver);
            serviceHandle = new Application.Host.HostStarter(() => applicationBusBuilder.NewContainer(), settingsResolver);
        }
        
        public void Dispose()
        {
            serviceHandle?.Dispose();
        }
    }
}