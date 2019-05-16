using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace WebAPI.Infrastructure.Configurations
{
    public static class GlobalExceptionHandlerConfiguration
    {
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(new ExceptionHandlerOptions
            {
                ExceptionHandler = async context =>
                {
                    IExceptionHandlerFeature exceptionHandler = context.Features.Get<IExceptionHandlerFeature>();
                    if (exceptionHandler != null)
                    {
                        Log.Debug(exceptionHandler.Error, string.Empty);
                        await context.Response.WriteAsync(exceptionHandler.Error.Message).ConfigureAwait(false);
                    }
                }
            });

            return app;
        }
    }
}