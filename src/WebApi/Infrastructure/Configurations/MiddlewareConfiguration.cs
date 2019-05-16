using Microsoft.AspNetCore.Builder;
using WebAPI.Infrastructure.Middleware;

namespace WebAPI.Infrastructure.Configurations
{
    public static class MiddlewareConfiguration
    {
        public static IApplicationBuilder UseResponseHandler(this IApplicationBuilder app)
        {
            app.UseMiddleware<ResponseHandler>();

            return app;
        }
    }
}