namespace Willow.Query.Host
{
    using System;
    using System.Reflection;
    using Autofac;
    using Denormalizer.Common;
    using Denormalizer.MongoDB;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Swashbuckle.AspNetCore.Swagger;

    public class WebHostStartup
    {
        public IConfiguration Configuration { get; }
        internal static Func<string, string> SettingsResolver { get; set; }

        public WebHostStartup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        //.AllowCredentials()
                        .WithExposedHeaders(
                            "ETag",
                            "If-Match",
                            "If-None-Match",
                            "Location",
                            "X-Pagination",
                            "x-correlation-id",
                            "x-causation-id"));
            });

            services
                .AddMvc()
                .AddApplicationPart(typeof(Query.Module).GetTypeInfo().Assembly)
                .AddControllersAsServices()
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
                });

            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new Info {Title = "Willow.Query", Version = "v1"}); });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger("Willow.Query");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(builder => { });

            app
                .UseMvc()
                .UseSwagger()
                .UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Willow.Query V1"); });
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            RegisterDocumentStore(builder);
            builder.RegisterModule<Query.Module>();
            builder.RegisterModule<Denormalizer.Repositories.Module>();
        }

        private static void RegisterDocumentStore(ContainerBuilder builder)
        {
            var serverAddress = SettingsResolver("MongoDb.ServerAddress");
            var serverPort = int.Parse(SettingsResolver("MongoDb.ServerPort"));
            var databaseName = SettingsResolver("MongoDb.DatabaseName");
            var userName = SettingsResolver("MongoDb.UserName");
            var userPassword = SettingsResolver("MongoDb.UserPassword");

            var settings = new MongoDbConnectionSettings(serverAddress, serverPort, databaseName, userName, userPassword);

            builder.Register(c => new MongoDb(settings)).As<IDocumentStore>();
        }
    }
}