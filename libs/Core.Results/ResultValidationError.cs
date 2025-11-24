namespace Core.Results;

public class ResultValidationError
{
    public string Identifier { get; set; }

    public string ErrorMessage { get; set; }

    public string ErrorCode { get; set; }

    public ValidationSeverity Severity { get; set; }

    public ResultValidationError()
    {
    }

    public ResultValidationError(string errorMessage)
    {
        ErrorMessage = errorMessage;
    }

    public ResultValidationError(string identifier, string errorMessage, string errorCode, ValidationSeverity severity)
    {
        Identifier = identifier;
        ErrorMessage = errorMessage;
        ErrorCode = errorCode;
        Severity = severity;
    }
}
