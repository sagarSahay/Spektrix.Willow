namespace Willow.API.Host
{
    using System;
    using System.Reflection;
    using Autofac;
    using Filters;
    using MassTransit;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using Swashbuckle.AspNetCore.Swagger;

    public class WebHostStartup
    {
        public WebHostStartup(IConfiguration configuration)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        internal static Func<string, string> SettingsResolver { get; set; }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvc(opt =>
                {
                    opt.Filters.Add<ValidateModelStateAttribute>();
                })
                .AddApplicationPart(typeof(API.Module).GetTypeInfo().Assembly)
                .AddControllersAsServices()
                .AddJsonOptions(options => { options.SerializerSettings.Formatting = Formatting.Indented; });

            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new Info {Title = "Willow.API", Version = "v1"}); });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app
                .UseMvc()
                .UseSwagger()
                .UseSwaggerUI(c=> c.SwaggerEndpoint("/swagger/v1/swagger.json", "Willow.API.V1"));
        }
        
        public void ConfigureContainer(ContainerBuilder builder)
        {
            RegisterMassTransit(builder);
            //builder.RegisterModule<Willow.API.Module>();
        }

        private void RegisterMassTransit(ContainerBuilder builder)
        {
            var host = SettingsResolver("RabbitMqHost");
            var username = SettingsResolver("RabbitMqUser");
            var password = SettingsResolver("RabbitMqPassword");
            var commandQueue = SettingsResolver("CommandQueue");
            builder.Register(c =>
                {
                    var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
                    {
                        sbc.Host(new Uri(host), h =>
                        {
                            h.Username(username);
                            h.Password(password);
                        });

                    });

                    return bus.GetSendEndpoint(new Uri($"{host}/{commandQueue}")).Result;
                })
                .As<ISendEndpoint>()
                .SingleInstance();
        }
    }
}