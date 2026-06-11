using AutoMapper;
using CloudinaryDotNet;
using ETS.Application.Abstraction.Common;
using ETS.Application.Behaviours;
using ETS.Domain.AppConfig;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace ETS.Application
{
    public static class DependencyInjection
    {
        /// <summary>
        /// Configure application services by registering them from the executing assembly
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddApplicationDependency(configuration, typeof(DependencyInjection).Assembly);
            services.Configure<DefaultAdminUserSetupOptions>(configuration.GetSection("DefaultAdminUserSetup"));
            services.Configure<PaystackConfigOptions>(configuration.GetSection("PaystackServiceConfig"));
            
            return services;
        }


        public static IServiceCollection AddApplicationDependency(this IServiceCollection services,
            IConfiguration configuration, Assembly assembly)
        {
            services.AddMediatR(config =>
            {
            config.RegisterServicesFromAssemblies(assembly);

                config.AddOpenBehavior(typeof(LoggingBehaviour<,>));
                config.AddOpenBehavior(typeof(UnhandledExceptionBehaviour<,>));
                config.AddOpenBehavior(typeof(ValidationBehaviour<,>));

            });

            services.AddValidatorsFromAssembly(assembly, includeInternalTypes: true);
            services.AutoRegisterBackgroundJobs(assembly);

            return services;            
        }


        private static void AutoRegisterBackgroundJobs(this IServiceCollection services, Assembly assembly)
        {
            var hostedServiceTypes = assembly
                .GetTypes()
                .Where(t => typeof(IHostedService).IsAssignableFrom(t) &&
                t is { IsAbstract: false, IsClass: true });

            foreach(var serviceType  in hostedServiceTypes)
            {
                services.AddSingleton(typeof(IHostedService), serviceType);
            }
        }


    }
}
