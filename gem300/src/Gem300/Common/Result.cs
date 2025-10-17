namespace Gem300.Common;

public class Result
{
    public bool IsSuccess { get; }
    public string? Error { get; }

    protected Result(bool ok, string? error)
    {
        IsSuccess = ok;
        Error = error;
    }

    public static Result Ok() => new(true, null);
    public static Result Fail(string error) => new(false, error);
}

public class Result<T> : Result
{
    public T? Value { get; }
    private Result(bool ok, T? value, string? error) : base(ok, error)
    {
        Value = value;
    }

    public static Result<T> Ok(T value) => new(true, value, null);
    public new static Result<T> Fail(string error) => new(false, default, error);
}
