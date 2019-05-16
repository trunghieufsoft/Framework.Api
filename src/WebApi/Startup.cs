using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using System;
using System.Text;
using Common.Core.Timing;
using Database.EntityFrameworkCore;
using Service.Services;
using Service.Services.Abstractions;
using WebAPI.Infrastructure.Configurations;

namespace WebAPI
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
            var logDB = _configuration.GetConnectionString("DefaultConnection");
            var logTable = "TBL_LOG_WORK";
            var options = new ColumnOptions();
            options.Store.Add(StandardColumn.LogEvent);
            options.Store.Remove(StandardColumn.MessageTemplate);
            options.Store.Remove(StandardColumn.Properties);
            options.LogEvent.DataLength = 2048;
            options.PrimaryKey = options.TimeStamp;
            options.TimeStamp.NonClusteredIndex = true;
            Log.Logger = new Serilog.LoggerConfiguration()
                .MinimumLevel.Information()
                //.WriteTo.RollingFile("Log-{Date}.txt", retainedFileCountLimit: 2)
                .WriteTo.MSSqlServer(connectionString: logDB, tableName: logTable, columnOptions: options, restrictedToMinimumLevel: LogEventLevel.Information)
                .CreateLogger();

            // Setting clock provider, using local time
            Clock.Provider = new UtcClockProvider();
            Log.Information("Web Start");
        }

        public IContainer ApplicationContainer { get; private set; }
        public AutofacServiceProvider provider;

        private void OnShutdown()
        {
            Log.Information("Shutdown");
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<ILogService, LogService>();
            services.AddGzipCompression();
            services.AddCorsConfig();
            services.AddTransient<DbInitializer>();
            services.AddServiceMvc();
            services.AddDbContext<APIDbContext>(options =>
                options.UseSqlServer(_configuration.GetConnectionString("DefaultConnection")));

            if (_configuration.GetValue<bool>("Config:EnableSwagger"))
            {
                services.AddSwaggerDocumentation();
            }
            services.AddServices(_configuration);
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidAudience = _configuration["Jwt:Issuer"],
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]))
                };
            });
            // Create the container builder.
            ContainerBuilder builder = new ContainerBuilder();
            builder.Populate(services);
            builder.RegisterModule<Database.ModuleInit>();
            builder.RegisterModule<Service.ModuleInit>();

            ApplicationContainer = builder.Build();
            provider = new AutofacServiceProvider(ApplicationContainer);
            return provider;
        }

        public void Configure(IApplicationBuilder app, DbInitializer dbInitializer)
        {
            var applicationLifetime = app.ApplicationServices.GetRequiredService<IApplicationLifetime>();
            applicationLifetime.ApplicationStopping.Register(OnShutdown);
            IHostingEnvironment env = app.ApplicationServices.GetService<IHostingEnvironment>();
            ILoggerFactory loggerFactory = app.ApplicationServices.GetService<ILoggerFactory>();

            loggerFactory.AddLoggerFactory(_configuration);
            app.UseGlobalExceptionHandler();
            app.UseGzipCompression();
            app.UseCorsConfig();
            app.UseResponseHandler();
            app.UseAuthentication();
            app.UseServiceMvc();
            dbInitializer.Seed().Wait();

            if (_configuration.GetValue<bool>("Config:EnableSwagger"))
            {
                app.UseSwaggerDocumentation();
            }
        }
    }
}