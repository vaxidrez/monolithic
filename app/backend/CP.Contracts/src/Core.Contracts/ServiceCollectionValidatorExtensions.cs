using System.Reflection;

using CP.Core.Contracts.Core;

using Microsoft.Extensions.DependencyInjection;

namespace CP.Core.Contracts;

public static class ServiceCollectionValidatorExtensions
{
    public static IServiceCollection AddModuleValidators(this IServiceCollection services, Assembly assembly)
    {
        var validatorTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract &&
                        t.GetInterfaces().Any(i =>
                            i.IsGenericType &&
                            i.GetGenericTypeDefinition() == typeof(IValidator<>)))
            .ToList();

        Console.WriteLine($"🔍 Registrando {validatorTypes.Count} validators (Assembly: {assembly.GetName().Name})");

        foreach (var validatorType in validatorTypes)
        {
            var validatorInterface = validatorType.GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValidator<>));

            services.AddScoped(validatorInterface, validatorType);
            Console.WriteLine($"   ✅ Validator: {validatorType.Name}");
        }

        return services;
    }
}