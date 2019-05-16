using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Security.Authentication;
using WebAPI.Infrastructure.Services;

namespace WebAPI.Infrastructure.Configurations
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient<IForwarderService, ForwarderService>()
                .ConfigurePrimaryHttpMessageHandler(sp => new HttpClientHandler
                {
                    AllowAutoRedirect = false,
                    ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true,
                    SslProtocols = configuration.GetValue<bool>("Config:SSLTrustedAlways")
                        ? SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12
                        : SslProtocols.None
                });

            return services;
        }
    }
}