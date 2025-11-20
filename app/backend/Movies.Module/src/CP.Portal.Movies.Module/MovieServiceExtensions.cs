using CP.Portal.Movies.Module.Data;
using CP.Portal.Movies.Module.Data.Repositories;
using CP.Portal.Movies.Module.Data.Seedings;
using CP.Portal.Movies.Module.Services;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CP.Portal.Movies.Module;

public static class MovieServiceExtensions
{
    public static IServiceCollection AddMovieServices(this IServiceCollection services, ConfigurationManager config)
    {
        string? connectionString = config.GetConnectionString("MoviesConnectionString");

        services.AddDbContext<MovieDbContext>(opt =>
        {
            opt.UseNpgsql(connectionString)
               .UseSnakeCaseNamingConvention()
               // Orquestador de seeding async (solo relaciones y Movies)
               .UseAsyncSeeding(async (db, isFirstRun, ct) =>
               {
                   var dbcontext = (MovieDbContext)db;
                   // 1) Movies
                   var moviesMap = await MoviesAsyncSeeder.SeedAsync(dbcontext, ct);

                   // 2) Relaciones
                   await MovieGenresAsyncSeeder.SeedAsync(dbcontext, moviesMap, ct);
                   await MovieCastsAsyncSeeder.SeedAsync(dbcontext, moviesMap, ct);
                   await MovieCrewsAsyncSeeder.SeedAsync(dbcontext, moviesMap, ct);
               });
        });

        services.AddScoped<IMovieRepository, EfMovieRepository>();
        services.AddScoped<IMovieService, MovieService>();

        return services;
    }
}