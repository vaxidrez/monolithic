using FastEndpoints;

namespace CP.Core.Contracts.Core;

/// <summary>
/// Pre-processor that automatically validates requests using registered IValidator<TRequest>.
/// </summary>
public sealed class ValidationPreProcessor<TRequest> : IPreProcessor<TRequest>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    // ✅ FIX: Constructor debe aceptar IEnumerable, no IValidator directamente
    public ValidationPreProcessor(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators ?? Enumerable.Empty<IValidator<TRequest>>();
    }

    public Task PreProcessAsync(IPreProcessorContext<TRequest> context, CancellationToken ct)
    {
        if (context.Request is null)
            return Task.CompletedTask;

        // ✅ Solo validar si hay validators registrados
        if (!_validators.Any())
            return Task.CompletedTask;

        var errors = _validators
            .SelectMany(v => v.Validate(context.Request))
            .ToList();

        if (errors.Count > 0)
        {
            foreach (var error in errors)
            {
                context.ValidationFailures.Add(new FluentValidation.Results.ValidationFailure(
                    error.PropertyName,
                    error.ErrorMessage));
            }
        }

        return Task.CompletedTask;
    }
}