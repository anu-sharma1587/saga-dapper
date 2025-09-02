namespace HotelManagement.BuildingBlocks.Common.Results;

public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string Error { get; }
    protected object? Value { get; }

    protected Result(bool isSuccess, string error, object? value = null)
    {
        IsSuccess = isSuccess;
        Error = error;
        Value = value;
    }

    public static Result Success() => new(true, string.Empty);
    public static Result<T> Success<T>(T value) => new(value, true, string.Empty);
    public static Result Failure(string error) => new(false, error);
    public static Result<T> Failure<T>(string error) => new(default, false, error);
}

public class Result<T> : Result
{
    public T? Value => (T?)base.Value;

    protected internal Result(T? value, bool isSuccess, string error)
        : base(isSuccess, error, value)
    {
    }
}
