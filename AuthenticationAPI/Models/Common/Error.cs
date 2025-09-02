namespace AuthenticationAPI.Models.Common;

public class Error
{
    public string? ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }
    public ErrorType ErrorType { get; set; }
}

public enum ErrorType
{
    Validation,
    NotFound,
    Conflict,
    Unauthorized,
    InternalError,
}