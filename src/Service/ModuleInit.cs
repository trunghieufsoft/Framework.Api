using Autofac;
using Service.Services.Abstractions;

namespace Service
{
    public class ModuleInit : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Business services
            builder.RegisterAssemblyTypes(GetType().Assembly)
                .Where(type => typeof(BaseService).IsAssignableFrom(type))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }
    }
}