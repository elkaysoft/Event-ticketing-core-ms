using CloudinaryDotNet;
using ETS.Domain.AppConfig;
using ETS.Domain.Contracts;
using ETS.Infrastructure.Authentication;
using ETS.Infrastructure.ExternalServices;
using ETS.Infrastructure.Persistence.DbContexts;
using ETS.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Reflection;
using System.Text.Json.Serialization;

namespace ETS.Infrastructure
{
    public static class DependencyInjection
    {
        private static string CorsPolicyName = "EVSCoreCorsPolicy";
        public static IServiceCollection ConfigureInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration, IWebHostEnvironment env)
            => services
            .ConfigureAppServicesInfrastructure(configuration, Assembly.GetExecutingAssembly())
            .AutoRegisterRepositoriesInAssembly(Assembly.GetExecutingAssembly())
            .AddPersistence(configuration)
            .AddAuthenticationSection(configuration);



        private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddServiceDbContexts<WriteApplicationDbContext, ReadApplicationDbContext>(
                configuration,
                writeConnectionString: "WriteDbConnection",
                readConnectionString: "ReadDbConnection");

            return services;
        }

        private static IServiceCollection AddServiceDbContexts<TWriteContext, TReadContext>(
            this IServiceCollection services,
            IConfiguration configuration,
            string writeConnectionString = "WriteDbConnection",
            string readConnectionString = "ReadDbConnection")
            where TWriteContext : DbContext, IWriteApplicationDbContext, IUnitOfWork
            where TReadContext : DbContext, IReadApplicationDbContext
        {
            services.AddDbContext<TWriteContext>(options =>
                options.SetDb<TWriteContext>(configuration, writeConnectionString));

            services.AddScoped<IWriteApplicationDbContext>(sp => sp.GetRequiredService<TWriteContext>());
            services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<TWriteContext>());
            
            services.AddDbContext<TReadContext>(options =>
                options.SetDb<TReadContext>(configuration, readConnectionString));
            services.AddScoped<IReadApplicationDbContext>(sp => sp.GetRequiredService<TReadContext>());

            return services;
        }

        private static DbContextOptionsBuilder SetDb<T>(
            this DbContextOptionsBuilder options,
            IConfiguration configuration,
            string connectionString = "WriteDbConnection",
            Assembly? migrationAssembly = null)
        {
            var assemblyName = (migrationAssembly ?? typeof(T).Assembly).FullName;

            return options.UseSqlServer(
                configuration.GetConnectionString(connectionString) ?? "",
                s => s.CommandTimeout((int)TimeSpan.FromMinutes(10).TotalSeconds)
                .MigrationsAssembly(assemblyName)
                .EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null));
        }

        private static IServiceCollection ConfigureAppServicesInfrastructure(this IServiceCollection services, 
            IConfiguration configuration,
            Assembly assembly)
        {
            var originStr = configuration.GetSection("AllowedOrigins").Get<string>();
            var allowedOrigins = string.IsNullOrEmpty(originStr)
                ? []
                : originStr.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            services.AddControllers()
                .AddJsonOptions(opt => opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()))
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.SuppressModelStateInvalidFilter = false;

                    options.InvalidModelStateResponseFactory = context =>
                    {
                        var httpContext = context.HttpContext;
                        var path = httpContext.Request.Path;

                        var message = string.Join(", ", context.ModelState.Values.SelectMany(a => a.Errors)
                            .Select(e => e.ErrorMessage));

                        var problemDetails = new ValidationProblemDetails
                        {
                            Title = "Invalid Request",
                            Status = StatusCodes.Status400BadRequest,
                            Detail = message,
                            Instance = path,
                            Type = "InvalidRequest"
                        };

                        return new BadRequestObjectResult(problemDetails);
                    };
                });

            services.Configure<MvcOptions>(options =>
            {
                options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = false;
            });

            services.AddEndpointsApiExplorer();

            services.AddCors(options =>
            {
                options.AddPolicy(name: CorsPolicyName, builder =>
                {
                    builder.WithOrigins(allowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
                });
            });
            services.AddHttpContextAccessor();

            return services;
        }

        private static IServiceCollection AutoRegisterRepositoriesInAssembly(this IServiceCollection services,
            Assembly assembly)
        {
            var repositoryTypes = assembly.GetTypes()
                .Where(t => t is { IsClass: true, IsAbstract: false } &&
                t.GetInterfaces().Any(i => t.Name.EndsWith("Repository")));

            foreach(var repositoryType in repositoryTypes)
            {
                var interfaceType = repositoryType.GetInterfaces().FirstOrDefault(i => i.Name.EndsWith("Repository"));
                if(interfaceType is not null)
                {
                    services.AddScoped(interfaceType, repositoryType);
                }
            }
            return services;
        }

        public static void RunAppPipeline<T, TDb, TDbContext>(this WebApplication app, Func<IServiceScope, bool> callback) where TDbContext : IDbContext where TDb : DbContext
        {
            app.UseRouting();
            app.UseCors(CorsPolicyName);

            app.UseAuthentication();
            app.UseAuthorization();

            using(var scope = app.Services.CreateScope())
            {
                scope.DbMigrate<T, TDb, TDbContext>();
                callback(scope);
            }

            app.Run();
        } 


        /// <summary>
        /// Migrates the database to the latest version.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TDb"></typeparam>
        /// <typeparam name="TIDbContext"></typeparam>
        /// <param name="service"></param>
        public static void DbMigrate<T, TDb, TIDbContext>(this IServiceScope service) where TDb : DbContext where TIDbContext : notnull
        {
            var logger = service.ServiceProvider.GetRequiredService<ILogger<T>>();
            try
            {
                var db = service.ServiceProvider.GetRequiredService<TIDbContext>() as TDb;
                db?.Database.Migrate();
            }
            catch(Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
        }


        private static IServiceCollection AddAuthenticationSection(this IServiceCollection services,
            IConfiguration configuration)
        {                           
            services.Configure<AuthenticationOptions>(configuration.GetSection("Authentication"));            
            services.AddScoped<IUserContext, UserContext>();
            services.AddScoped<ITokenService, TokenService>();

            services.Configure<CloudinarySettings>(configuration.GetSection("Cloudinary"));
            services.AddSingleton(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<CloudinarySettings>>().Value;
                var account = new Account(settings.CloudName, settings.ApiKey, settings.ApiSecret);
                return new Cloudinary(account);
            });

            services.AddSingleton<IDocumentService, DocumentService>();
            services.AddHttpClient<IMicroserviceHttpClient, MicroserviceHttpClient>();
            services.AddSingleton<IPaystackService, PaystackService>();

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer();

            services.ConfigureOptions<JwtBearerOptionsSetup>();

            return services;
        }

    }
}
