

using System.Reflection;
using System.Text;

using CP.Portal.Api.Middleware;
using CP.Portal.Movies.Module;
using CP.Portal.Movies.Module.Data;
using CP.Portal.Users.Module;
using CP.Portal.Users.Module.Data;

using FastEndpoints;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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

var jwtSecret = builder.Configuration["Auth:JwtSecret"]!;
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();




var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

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