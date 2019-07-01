﻿using System;

namespace Willow.API.Host.Console
{
    using System.Runtime.Loader;
    using System.Threading;
    using Console = System.Console;

    internal class Program
    {
        private static readonly string serviceTypeName = "Willow.API.Host.Console";
        private static readonly ManualResetEvent exitEvent = new ManualResetEvent(false);
        
        static void Main(string[] args)
        {
            try
            {
                var settingsResolver = ApplicationSettingsResolver.GetSettingsResolver();
                var baseUrl = settingsResolver("APIServiceUrl");
                
                using (var serviceHost = new HostStarter(settingsResolver, baseUrl, args))
                {
                    Console.WriteLine($"Service is ready on {baseUrl}{Environment.NewLine}Press Ctrl-C to exit");
                    AttachExitHandlers();
                    exitEvent.WaitOne();
                    Environment.Exit(0);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fatal error in starting {serviceTypeName} service. Exception {ex}");
                throw;
            }
        }
        
        private static void AttachExitHandlers()
        {
            AssemblyLoadContext.Default.Unloading += context =>
            {
                Console.WriteLine("Unloading...");
                exitEvent.Set();
            };

            Console.CancelKeyPress += (_, __) =>
            {
                Console.WriteLine("Exiting...");
                exitEvent.Set();
            };
        }
    }
}