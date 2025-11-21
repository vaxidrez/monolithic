namespace CP.Core.Contracts.Core;

public interface IValidator<TRequest>
{
    IEnumerable<ValidationError> Validate(TRequest request);
}