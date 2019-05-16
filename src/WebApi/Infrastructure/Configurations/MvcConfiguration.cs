using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using WebAPI.Infrastructure.Attributes;
using WebAPI.Infrastructure.Filters;

namespace WebAPI.Infrastructure.Configurations
{
    public static class MvcConfiguration
    {
        public static IServiceCollection AddServiceMvc(this IServiceCollection services)
        {
            services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
            });

            services
                .AddMvc(options =>
                {
                    options.Filters.Add(new ValidateModelStateAttribute());
                    options.Filters.Add(typeof(HttpGlobalExceptionFilter));
                })
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    options.SerializerSettings.Formatting = Formatting.Indented;
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                });

            return services;
        }

        public static IApplicationBuilder UseServiceMvc(this IApplicationBuilder app)
        {
            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller}/{action=Index}/{id?}");
            });

            return app;
        }
    }
}