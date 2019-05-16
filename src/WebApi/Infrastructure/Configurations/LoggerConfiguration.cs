using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;

namespace WebAPI.Infrastructure.Configurations
{
    public static class LoggerConfiguration
    {
        public static ILoggerFactory AddLoggerFactory(this ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            #pragma warning disable CS0618 // Type or member is obsolete

            loggerFactory.AddConsole(configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            #pragma warning restore CS0618 // Type or member is obsolete

            loggerFactory.AddSerilog();

            return loggerFactory;
        }
    }
}