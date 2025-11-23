using System.Text.Json.Serialization;

namespace CP.Core.Contracts.Core;

public class Result<T> : IResult
{
    public T Value { get; }

    [JsonIgnore]
    public Type ValueType => typeof(T);

    public ResultStatus Status { get; protected set; }

    public bool IsSuccess => Status == ResultStatus.Ok;

    public string SuccessMessage { get; protected set; } = string.Empty;

    public string CorrelationId { get; protected set; } = string.Empty;

    public IEnumerable<string> Errors { get; protected set; } = new List<string>();

    public List<ResultValidationError> ValidationErrors { get; protected set; } = new List<ResultValidationError>();

    protected Result()
    {
    }

    public Result(T value)
    {
        Value = value;
    }

    protected internal Result(T value, string successMessage)
        : this(value)
    {
        SuccessMessage = successMessage;
    }

    protected Result(ResultStatus status)
    {
        Status = status;
    }

    public static implicit operator T(Result<T> result)
    {
        return result.Value;
    }

    public static implicit operator Result<T>(T value)
    {
        return new Result<T>(value);
    }

    public static implicit operator Result<T>(Result result)
    {
        return new Result<T>(default(T))
        {
            Status = result.Status,
            Errors = result.Errors,
            SuccessMessage = result.SuccessMessage,
            CorrelationId = result.CorrelationId,
            ValidationErrors = result.ValidationErrors
        };
    }

    public object GetValue()
    {
        return Value;
    }

    public PagedResult<T> ToPagedResult(PagedInfo pagedInfo)
    {
        return new PagedResult<T>(pagedInfo, Value)
        {
            Status = Status,
            SuccessMessage = SuccessMessage,
            CorrelationId = CorrelationId,
            Errors = Errors,
            ValidationErrors = ValidationErrors
        };
    }

    public static Result<T> Success(T value)
    {
        return new Result<T>(value);
    }

    public static Result<T> Success(T value, string successMessage)
    {
        return new Result<T>(value, successMessage);
    }

    public static Result<T> Error(params string[] errorMessages)
    {
        return new Result<T>(ResultStatus.Error)
        {
            Errors = errorMessages
        };
    }

    public static Result<T> Invalid(ResultValidationError validationError)
    {
        return new Result<T>(ResultStatus.Invalid)
        {
            ValidationErrors = { validationError }
        };
    }

    public static Result<T> Invalid(params ResultValidationError[] validationErrors)
    {
        return new Result<T>(ResultStatus.Invalid)
        {
            ValidationErrors = new List<ResultValidationError>(validationErrors)
        };
    }

    public static Result<T> Invalid(List<ResultValidationError> validationErrors)
    {
        return new Result<T>(ResultStatus.Invalid)
        {
            ValidationErrors = validationErrors
        };
    }

    public static Result<T> NotFound()
    {
        return new Result<T>(ResultStatus.NotFound);
    }

    public static Result<T> NotFound(params string[] errorMessages)
    {
        return new Result<T>(ResultStatus.NotFound)
        {
            Errors = errorMessages
        };
    }

    public static Result<T> Forbidden()
    {
        return new Result<T>(ResultStatus.Forbidden);
    }

    public static Result<T> Unauthorized()
    {
        return new Result<T>(ResultStatus.Unauthorized);
    }

    public static Result<T> Conflict()
    {
        return new Result<T>(ResultStatus.Conflict);
    }

    public static Result<T> Conflict(params string[] errorMessages)
    {
        return new Result<T>(ResultStatus.Conflict)
        {
            Errors = errorMessages
        };
    }

    public static Result<T> CriticalError(params string[] errorMessages)
    {
        return new Result<T>(ResultStatus.CriticalError)
        {
            Errors = errorMessages
        };
    }

    public static Result<T> Unavailable(params string[] errorMessages)
    {
        return new Result<T>(ResultStatus.Unavailable)
        {
            Errors = errorMessages
        };
    }
}



public class Result : Result<Result>
{
    public Result()
    {
    }

    protected internal Result(ResultStatus status)
        : base(status)
    {
    }

    public static Result Success()
    {
        return new Result();
    }

    public static Result SuccessWithMessage(string successMessage)
    {
        return new Result
        {
            SuccessMessage = successMessage
        };
    }

    public static Result<T> Success<T>(T value)
    {
        return new Result<T>(value);
    }

    public static Result<T> Success<T>(T value, string successMessage)
    {
        return new Result<T>(value, successMessage);
    }

    public new static Result Error(params string[] errorMessages)
    {
        return new Result(ResultStatus.Error)
        {
            Errors = errorMessages
        };
    }

    public static Result ErrorWithCorrelationId(string correlationId, params string[] errorMessages)
    {
        return new Result(ResultStatus.Error)
        {
            CorrelationId = correlationId,
            Errors = errorMessages
        };
    }

    public new static Result Invalid(ResultValidationError validationError)
    {
        return new Result(ResultStatus.Invalid)
        {
            ValidationErrors = { validationError }
        };
    }

    public new static Result Invalid(params ResultValidationError[] validationErrors)
    {
        return new Result(ResultStatus.Invalid)
        {
            ValidationErrors = new List<ResultValidationError>(validationErrors)
        };
    }

    public new static Result Invalid(List<ResultValidationError> validationErrors)
    {
        return new Result(ResultStatus.Invalid)
        {
            ValidationErrors = validationErrors
        };
    }

    public new static Result NotFound()
    {
        return new Result(ResultStatus.NotFound);
    }

    public new static Result NotFound(params string[] errorMessages)
    {
        return new Result(ResultStatus.NotFound)
        {
            Errors = errorMessages
        };
    }

    public new static Result Forbidden()
    {
        return new Result(ResultStatus.Forbidden);
    }

    public new static Result Unauthorized()
    {
        return new Result(ResultStatus.Unauthorized);
    }

    public new static Result Conflict()
    {
        return new Result(ResultStatus.Conflict);
    }

    public new static Result Conflict(params string[] errorMessages)
    {
        return new Result(ResultStatus.Conflict)
        {
            Errors = errorMessages
        };
    }

    public new static Result Unavailable(params string[] errorMessages)
    {
        return new Result(ResultStatus.Unavailable)
        {
            Errors = errorMessages
        };
    }

    public new static Result CriticalError(params string[] errorMessages)
    {
        return new Result(ResultStatus.CriticalError)
        {
            Errors = errorMessages
        };
    }
}