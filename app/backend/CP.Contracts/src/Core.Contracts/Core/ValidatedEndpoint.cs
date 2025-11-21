using FastEndpoints;


using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace CP.Core.Contracts.Core;

public abstract class ValidatedEndpoint<TRequest> : Endpoint<TRequest>
    where TRequest : notnull
{
    public sealed override async Task HandleAsync(TRequest req, CancellationToken ct)
    {
        var validators = HttpContext.RequestServices.GetServices<IValidator<TRequest>>()
                         ?? Enumerable.Empty<IValidator<TRequest>>();

        var errors = validators.SelectMany(v => v.Validate(req)).ToList();

        if (errors.Count > 0)
        {
            // Construir diccionario: campo -> lista de mensajes (solo uno normalmente)
            var dict = errors
                .GroupBy(e => ToCamel(e.PropertyName))
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.ErrorMessage).ToArray()
                );

            var payload = new
            {
                statusCode = 400,
                message = "Validation failed",
                errors = dict
            };

            // await SendAsync(payload, 400, cancellation: ct);
            //await SendAsync(payload, 400, ct);
            HttpContext.Response.StatusCode = 400;
            await HttpContext.Response.WriteAsJsonAsync(payload, ct);
            return;
        }

        await OnValidatedAsync(req, ct);
    }

    protected abstract Task OnValidatedAsync(TRequest req, CancellationToken ct);

    private static string ToCamel(string s)
        => string.IsNullOrEmpty(s) || char.IsLower(s[0])
            ? s
            : char.ToLowerInvariant(s[0]) + s.Substring(1);
}
//public abstract class ValidatedEndpoint<TRequest> : Endpoint<TRequest>
//    where TRequest : notnull
//{
//    // CS0507 FIX: Method in base is public; override must keep 'public'.
//    // Sealed to prevent further overrides; use OnValidatedAsync for custom logic.
//    public sealed override async Task HandleAsync(TRequest req, CancellationToken ct)
//    {
//        var validators = HttpContext.RequestServices.GetServices<IValidator<TRequest>>()
//                         ?? Enumerable.Empty<IValidator<TRequest>>();

//        var errors = validators
//            .SelectMany(v => v.Validate(req))
//            .ToList();

//        if (errors.Count > 0)
//        {
//            foreach (var e in errors)
//                AddError(e.PropertyName, e.ErrorMessage);

//            await Send.ErrorsAsync(cancellation: ct);
//            return;
//        }

//        await OnValidatedAsync(req, ct);
//    }

//    // Implement endpoint logic here (runs only if validation passed).
//    protected abstract Task OnValidatedAsync(TRequest req, CancellationToken ct);
//}

//public abstract class ValidatedEndpoint<TRequest> : Endpoint<TRequest>
//    where TRequest : notnull
//{
//    public override void Configure()
//    {
//        ConfigureEndpoint();
//        Console.WriteLine($"🔍 Configurando ValidatedEndpoint para {typeof(TRequest).Name}");
//        PreProcessor<ValidationPreProcessorInline<TRequest>>();
//    }

//    protected abstract void ConfigureEndpoint();
//}

//// ✅ Pre-processor con constructor explícito para que DI pueda construirlo
//public sealed class ValidationPreProcessorInline<TRequest> : IPreProcessor<TRequest>
//    where TRequest : notnull
//{
//    // ✅ Constructor explícito sin parámetros (requerido por FastEndpoints DI)
//    public ValidationPreProcessorInline()
//    {
//        Console.WriteLine($"✅ ValidationPreProcessorInline<{typeof(TRequest).Name}> construido");
//    }

//    public Task PreProcessAsync(IPreProcessorContext<TRequest> context, CancellationToken ct)
//    {
//        Console.WriteLine($"🔍 PreProcessAsync ejecutándose para {typeof(TRequest).Name}");

//        if (context.Request is null)
//        {
//            Console.WriteLine($"⚠️ Request es null para {typeof(TRequest).Name}");
//            return Task.CompletedTask;
//        }

//        var validators = context.HttpContext.RequestServices.GetServices<IValidator<TRequest>>();
//        Console.WriteLine($"🔍 Validators encontrados para {typeof(TRequest).Name}: {validators.Count()}");

//        if (!validators.Any())
//            return Task.CompletedTask;

//        var errors = validators
//            .SelectMany(v => v.Validate(context.Request))
//            .ToList();

//        Console.WriteLine($"🔍 Errores de validación encontrados: {errors.Count}");

//        if (errors.Count > 0)
//        {
//            foreach (var error in errors)
//            {
//                Console.WriteLine($"   ❌ {error.PropertyName}: {error.ErrorMessage}");
//                context.ValidationFailures.Add(new FluentValidation.Results.ValidationFailure(
//                    error.PropertyName,
//                    error.ErrorMessage));
//            }
//        }

//        return Task.CompletedTask;
//    }
//}