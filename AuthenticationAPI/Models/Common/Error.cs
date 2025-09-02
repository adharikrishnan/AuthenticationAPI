namespace AuthenticationAPI.Models.Common;

public class Error(string errorCode, string errorMessage, ErrorType errorType)
{
    public string? ErrorCode { get; set; } = errorCode;
    public string? ErrorMessage { get; set; } = errorMessage;
    public ErrorType ErrorType { get; set; } = errorType;
}

public enum ErrorType
{
    Validation,
    NotFound,
    Conflict,
    Unauthorized,
    InternalError,
}