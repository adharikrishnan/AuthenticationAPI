namespace AuthenticationAPI.Models.Common;

public sealed class Result<T>
{
    public Result(T data)
    {
        IsSuccess = true;
        Data = data;
    }

    public Result(string errorCode, string errorMessage, ErrorType errorType)
    {
        IsSuccess = false;
        Error = new Error(errorCode, errorMessage, errorType);
    }
    
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public Error? Error { get; set; }
    
}

public sealed class Result
{
    public Result()
    {
        IsSuccess = true;
    }
    
    public Result(string errorCode, string errorMessage, ErrorType errorType)
    {
        IsSuccess = false;
        Error = new Error(errorCode, errorMessage, errorType);
    }
    
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public Error? Error { get; set; }
}