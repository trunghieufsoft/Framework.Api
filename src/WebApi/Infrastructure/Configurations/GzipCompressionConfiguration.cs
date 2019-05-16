using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using System.IO.Compression;

namespace WebAPI.Infrastructure.Configurations
{
    public static class GzipCompressionConfiguration
    {
        public static IServiceCollection AddGzipCompression(this IServiceCollection services)
        {
            services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal);
            services.AddResponseCompression();

            return services;
        }

        public static IApplicationBuilder UseGzipCompression(this IApplicationBuilder app)
        {
            app.UseResponseCompression();
            return app;
        }
    }
}