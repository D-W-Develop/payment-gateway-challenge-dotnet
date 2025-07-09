namespace PaymentGateway.Api.Shared;

public class Result<T>
{
    public bool Success { get;}
    public T? Value { get; }
    public string? Error { get; }

    public Result(T value)
    {
        Success = true;
        Value = value;
    }

    public Result(string error)
    {
        Success = false;
        Error = error;
    }
}