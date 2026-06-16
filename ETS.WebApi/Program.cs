using ETS.Application;
using ETS.Application.Abstraction.Common;
using ETS.Domain.Contracts;
using ETS.Infrastructure;
using ETS.Infrastructure.Persistence.DbContexts;
using ETS.WebApi.Extensions;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
var assembly = Assembly.GetExecutingAssembly();

IWebHostEnvironment env = builder.Environment;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

builder.ConfigureDefaultSettings();

builder.Services
    .ConfigureInfrastructureServices(builder.Configuration, env)
    .ConfigureApplicationServices(builder.Configuration)
    .ConfigurePresentationSettings(assembly, builder.Configuration);

var app = builder.Build();

if (!app.Environment.IsProduction())
{
    app.UseDeveloperExceptionPage();
}

app.WebAppBuilderPipelineBuilder();
app.RunAppPipeline<Program, WriteApplicationDbContext, IWriteApplicationDbContext>(scope =>
{
    DbInitializer.Seed(scope);
    return true;
});
