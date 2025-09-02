namespace AuthenticationAPI.Models.Common;

public sealed class Result<T>
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public Error? Error { get; set; }
}

public sealed class Result
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    Error? Error { get; set; }
}