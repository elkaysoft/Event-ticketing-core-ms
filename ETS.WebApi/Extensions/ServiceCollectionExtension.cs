using ETS.Domain.Common;
using Serilog;
using Serilog.Formatting.Compact;
using System.Reflection;
using Microsoft.OpenApi;
using Serilog.Events;

namespace ETS.WebApi.Extensions
{
    internal static class ServiceCollectionExtension
    {
        public static void ConfigureDefaultSettings(this WebApplicationBuilder builder, bool configureOpenTelemetry = true)
        {
            var serviceName = "ets-core-service";

            builder.Logging.ClearProviders();           
       
            // configure serilog
            builder.Host.UseSerilog((ctx, sp, loggerConfiguration) =>
            {
                loggerConfiguration
                    .ReadFrom.Configuration(ctx.Configuration)
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .MinimumLevel.Override("System", LogEventLevel.Warning)
                    .Enrich.FromLogContext()                    
                    .Enrich.WithProperty("ServiceName", serviceName)
                    .Enrich.WithProperty("LogType", "Log")
                    .WriteTo.Console(new LogSanitizer(new RenderedCompactJsonFormatter()))
                    .WriteTo.File(
                    "logs/app.log", 
                    rollingInterval: RollingInterval.Month,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {TraceId} {Message:1j}{NewLine}{Exception}");

            });
        }

        public static IServiceCollection ConfigurePresentationSettings(this IServiceCollection services, Assembly assembly,
            IConfiguration configuration)
        {
            services.AddEndpointsApiExplorer()
                .AddSwaggerGenWithAuth(assembly, configuration);

            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();

            return services;
        }

        private static IServiceCollection AddSwaggerGenWithAuth(this IServiceCollection services, Assembly assembly,  
            IConfiguration configuration)
        {
            var serviceName = "ets-core-service";

            services.AddSwaggerGen(o =>
            {
                o.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
                {
                    Title = serviceName,
                    Version = "v1",
                    Description = "Event Ticketing Core API"
                });
                o.CustomSchemaIds(id => id.FullName!.Replace('+', '-'));

                var baseDir = AppContext.BaseDirectory;
                var webApiXml = Path.Combine(baseDir, $"{assembly.GetName().Name}.xml");
                if (File.Exists(webApiXml))
                    o.IncludeXmlComments(webApiXml);
                var appAssembly = typeof(ETS.Application.DependencyInjection).Assembly;
                var appXml = Path.Combine(baseDir, $"{appAssembly.GetName().Name}.xml");
                if (File.Exists(appXml))
                    o.IncludeXmlComments(appXml);
                o.TagActionsBy(api =>
                {
                    if (api.GroupName != null) return [api.GroupName];
                    var routeValues = api.ActionDescriptor.RouteValues;
                    var controller = routeValues.TryGetValue("controller", out var c) ? c : null;
                    return controller != null ? [controller] : null; 
                });


                o.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "Enter your JWT token. \n\nExample: \"eyJhbciosjdIEDJksk\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header
                });

                o.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("Bearer", document)] = []
                });

            });

            return services;
        }

        public static IApplicationBuilder WebAppBuilderPipelineBuilder(this WebApplication app)
        {
            //if (!app.Environment.IsProduction())
            //{
            //    app.UseSwagger();
            //    app.UseSwaggerUI();
            //}

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseSerilogRequestLogging();
            app.UseExceptionHandler();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.MapControllers();

            return app;
        }

    }
}
