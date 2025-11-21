

using CP.Portal.Api.Middleware;
using CP.Portal.Movies.Module;
using CP.Portal.Movies.Module.Data;

using FastEndpoints;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMovieServices(builder.Configuration);
builder.Services.AddOpenApi();



builder.Services.AddFastEndpoints();




var app = builder.Build();

// Aplicar migraciones automáticamente al inicio
await using (var scope = app.Services.CreateAsyncScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<MovieDbContext>();
    await dbContext.Database.MigrateAsync();
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