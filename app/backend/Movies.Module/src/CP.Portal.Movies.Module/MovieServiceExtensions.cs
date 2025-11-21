using Core.MediatOR;
using Core.MediatOR.Contracts;

using CP.Core.Contracts.Core;

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



        services.AddMediatOR(typeof(MovieServiceExtensions).Assembly);

        // Register the open-generic pipeline behavior explicitly
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        // Register all IValidator<T> from this assembly (internal/public)
        //var asm = typeof(MovieServiceExtensions).Assembly;
        //foreach (var type in asm.GetTypes().Where(t => t.IsClass && !t.IsAbstract))
        //{
        //    var validatorInterfaces = type.GetInterfaces()
        //        .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValidator<>))
        //        .ToArray();

        //    foreach (var itf in validatorInterfaces)
        //    {
        //        services.AddScoped(itf, type);
        //    }
        //}



        // ✅ Auto-registrar validators
        var assembly = typeof(MovieServiceExtensions).Assembly;

        var validatorTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract &&
                        t.GetInterfaces().Any(i =>
                            i.IsGenericType &&
                            i.GetGenericTypeDefinition() == typeof(IValidator<>)))
            .ToList();

        Console.WriteLine($"🔍 Registrando {validatorTypes.Count} validators");

        foreach (var validatorType in validatorTypes)
        {
            var validatorInterface = validatorType.GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValidator<>));

            // Registrar IValidator<TRequest>
            services.AddScoped(validatorInterface, validatorType);
            Console.WriteLine($"   ✅ Validator: {validatorType.Name}");
        }

        // ✅ SOLUCIÓN DEFINITIVA: Registro MANUAL EXPLÍCITO del pre-processor
        // FastEndpoints 7.1.1 NO puede resolver genéricos cerrados con factory
    

        // Agrega más si tienes otros endpoints con validación:
        // services.AddScoped<ValidationPreProcessorInline<UpdateMoviePriceRequest>>();


        return services;
    }
}