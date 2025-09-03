using System.Net;
using AuthenticationAPI.Models.Common;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationAPI.Extensions;

public static class ApiResultExtensions
{
    public static IActionResult MatchApiResponse(this Result result, string message)
    {
        return result.IsSuccess 
            ? new OkObjectResult(message) 
            : new ObjectResult(GetProblemDetails(result.Error!));
    }
    
    public static IActionResult MatchApiResponse<T>(this Result<T> result)
    {
        return result.IsSuccess ? 
            new OkObjectResult(result.Data) :
            new ObjectResult(GetProblemDetails(result.Error!));
    }

    private static ProblemDetails GetProblemDetails(Error error)
    {
        return new ProblemDetails()
        {
            Title = error.GetTitle(),
            Status = (int)GetStatusCode(error),
            Detail = error.GetDetail(),
        };
    }

    private static HttpStatusCode GetStatusCode(this Error error) =>
        error.ErrorType switch
        {
            ErrorType.NotFound => HttpStatusCode.NotFound,
            ErrorType.Validation => HttpStatusCode.BadRequest,
            ErrorType.Conflict => HttpStatusCode.Conflict,
            ErrorType.Unauthorized => HttpStatusCode.Unauthorized,
            _ => HttpStatusCode.InternalServerError,
        };

    private static string GetDetail(this Error error) =>
        error.ErrorMessage ?? "An Unexpected error occured please contact support.";
    
    private static string GetTitle(this Error error) =>
        error.ErrorCode ?? "UNEXPECTED_ERROR";
}