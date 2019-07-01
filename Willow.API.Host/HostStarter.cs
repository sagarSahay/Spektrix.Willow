namespace Willow.API.Host
{
    using System;
    using Autofac.Extensions.DependencyInjection;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;

    public class HostStarter : IDisposable
    {
        private readonly IWebHost webHost;

        public HostStarter(Func<string, string> settingsResolver, string baseUri, string[] args)
        {
            if (settingsResolver == null) throw new ArgumentNullException(nameof(settingsResolver));
            if (string.IsNullOrWhiteSpace(baseUri)) throw new ArgumentNullException(nameof(baseUri));

            webHost = BuildWebHost(settingsResolver, baseUri, args);
            webHost.Start();
        }

        private static IWebHost BuildWebHost(Func<string, string> settingsResolver, string baseUri, string[] args)
        {
            WebHostStartup.SettingsResolver = settingsResolver;

            return WebHost.CreateDefaultBuilder(args)
                .UseKestrel()
                .ConfigureServices(s => s.AddAutofac())
                .UseStartup<WebHostStartup>()
                .UseUrls(baseUri)
                .Build();
        }

        public void Dispose()
        {
            webHost?.Dispose();
        }
    }
}