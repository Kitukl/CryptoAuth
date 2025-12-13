namespace CryptoAuth.BLL;

public class Result
{
    public bool isSuccess { get; set; }
    public string Errors { get; set; } = string.Empty;
}

public class Result<T> : Result
{
    public T Value { get; set; }
    public static Result<T> Success(T value) => new Result<T> { isSuccess = true, Value = value };
    public static Result<T> Failure(string error) => new Result<T> { isSuccess = false, Errors = error };
}