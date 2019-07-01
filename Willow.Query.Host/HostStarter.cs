namespace Willow.Query.Host
{
    using System;
    using Autofac.Extensions.DependencyInjection;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;

    public class HostStarter : IDisposable
    {
        private readonly IWebHost webHost;

        public HostStarter(Func<string, string> settingsResolver, string baseUrl, string[] args)
        {
            if (settingsResolver == null) throw new ArgumentNullException(nameof(settingsResolver));
            if (string.IsNullOrWhiteSpace(baseUrl)) throw new ArgumentNullException(nameof(baseUrl));

            webHost = BuildWebHost(settingsResolver, baseUrl, args);
            webHost.Start();
        }

        public void Dispose()
        {
            webHost?.Dispose();
        }

        private static IWebHost BuildWebHost(Func<string, string> settingsResolver, string baseUrl, string[] args)
        {
            WebHostStartup.SettingsResolver = settingsResolver;

            return WebHost.CreateDefaultBuilder(args)
                .UseKestrel()
                .ConfigureServices(services => services.AddAutofac())
                .UseStartup<WebHostStartup>()
                .UseUrls(baseUrl)
                .Build();
        }
    }
}