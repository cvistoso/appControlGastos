namespace ExpenseControl.Shared.Common;

public sealed class Result
{
    public bool IsSuccess { get; }
    public string? ErrorCode { get; }
    public string? Message { get; }

    private Result(bool isSuccess, string? errorCode = null, string? message = null)
    {
        IsSuccess = isSuccess;
        ErrorCode = errorCode;
        Message = message;
    }

    public static Result Success() => new(true);
    public static Result Failure(string errorCode, string? message = null) => new(false, errorCode, message);
}

public sealed class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? ErrorCode { get; }
    public string? Message { get; }

    private Result(bool isSuccess, T? value, string? errorCode = null, string? message = null)
    {
        IsSuccess = isSuccess;
        Value = value;
        ErrorCode = errorCode;
        Message = message;
    }

    public static Result<T> Success(T value) => new(true, value);
    public static Result<T> Failure(string errorCode, string? message = null) => new(false, default, errorCode, message);
}
