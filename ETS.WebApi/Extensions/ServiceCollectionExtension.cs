using ETS.Domain.Common;
using Serilog;
using Serilog.Formatting.Compact;
using System.Reflection;
using Microsoft.OpenApi;

namespace ETS.WebApi.Extensions
{
    internal static class ServiceCollectionExtension
    {
        public static void ConfigureDefaultSettings(this WebApplicationBuilder builder, bool configureOpenTelemetry = true)
        {
            var serviceName = "ets-core-service";

            builder.Logging.ClearProviders();
            builder.Services.AddLogging(o =>
            {
                o.ClearProviders();
                o.Configure(x => x.ActivityTrackingOptions = ActivityTrackingOptions.TraceId | ActivityTrackingOptions.SpanId);
            });

            builder.Logging.Configure(opt =>
            {
                opt.ActivityTrackingOptions = ActivityTrackingOptions.SpanId
                | ActivityTrackingOptions.TraceId
                | ActivityTrackingOptions.ParentId
                | ActivityTrackingOptions.Baggage
                | ActivityTrackingOptions.Tags;
            });

            builder.Logging.AddJsonConsole();

            // configure serilog
            builder.Host.UseSerilog((_, sp, loggerConfiguration) =>
            {
                loggerConfiguration.MinimumLevel.Information();
                loggerConfiguration.WriteTo.Console();

                loggerConfiguration
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("ServiceName", serviceName)
                    .Enrich.WithProperty("LogType", "Log")
                    .WriteTo.Console(new LogSanitizer(new RenderedCompactJsonFormatter()));

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
                    Description = "Enter your JWT toke. \n\nExample: \"eyJhbciosjdIEDJksk\"",
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
            if (!app.Environment.IsProduction())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseSerilogRequestLogging();
            app.UseExceptionHandler();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.MapControllers();

            return app;
        }

    }
}
