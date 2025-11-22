

using System.Reflection;

using CP.Portal.Api.Middleware;
using CP.Portal.Movies.Module;
using CP.Portal.Movies.Module.Data;
using CP.Portal.Users.Module;
using CP.Portal.Users.Module.Data;

using FastEndpoints;

using Microsoft.EntityFrameworkCore;

using Serilog;


var logger = Log.Logger = new LoggerConfiguration()
  .Enrich.FromLogContext()
  .WriteTo.Console()
  .CreateLogger();

logger.Information("Starting web host");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMovieServices(builder.Configuration);

List<Assembly> mediatRAssemblies = [typeof(Program).Assembly];

builder.Services.AddUserModuleServices(builder.Configuration, logger, mediatRAssemblies);

builder.Services.AddOpenApi();

builder.Services.AddFastEndpoints();


var app = builder.Build();

// Aplicar migraciones automáticamente al inicio
await using (var scope = app.Services.CreateAsyncScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<MovieDbContext>();
    await dbContext.Database.MigrateAsync();

    var dbContextUser = scope.ServiceProvider.GetRequiredService<UserDbContext>();
    await dbContextUser.Database.MigrateAsync();
}

//app.UseFastEndpoints();

app.UseFastEndpoints(config =>
{
    // Configuración global usando ShortNames
    config.Endpoints.ShortNames = true;

    // Para FastEndpoints 7.1.1, necesitas usar GlobalPreProcessorRegister
    // durante el registro de servicios, no aquí
});


app.UseMiddleware<ExceptionMiddleware>();



app.Run();


public partial class Program { }