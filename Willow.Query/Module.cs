namespace Willow.Query
{
    using System.Linq;
    using Autofac;
    using Microsoft.AspNetCore.Mvc;

    public class Module : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterTypes(ThisAssembly.GetTypes()
                    .Where(x =>  typeof(Controller).IsAssignableFrom(x))
                    .ToArray())
                .InstancePerLifetimeScope();

            base.Load(builder);
        }
    }
}