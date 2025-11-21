
namespace CP.Core.Contracts.Core;

public sealed record ValidationError(
    string PropertyName,
    string ErrorMessage
    );
