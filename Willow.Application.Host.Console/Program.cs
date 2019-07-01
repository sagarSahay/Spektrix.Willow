namespace Willow.Application.Host.Console
{
    using System;
    using System.Runtime.Loader;
    using System.Threading;

    internal class Program
    {
        private static readonly string serviceTypeName = "Willow.Application.Host.Console";
        private static readonly ManualResetEvent exitEvent = new ManualResetEvent(false);
        
        static void Main(string[] args)
        {
            try
            {
                var settingsResolver = ApplicationSettingsResolver.GetSettingsResolver();

                using (new ApplicationInProcessHost(settingsResolver))
                {
                    Console.WriteLine($"Service is ready{Environment.NewLine}Press Ctrl-C to exit");
                    AttachExitHandlers();
                    exitEvent.WaitOne();
                    Environment.Exit(0);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
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