using AuthenticationAPI.Models.Common;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationAPI.Extensions;

public static class ApiResultExtensions
{
    public static IActionResult MatchApiResponse<T>(this Result<T> result)
    {
        if (result.IsSuccess)
        {
            return new OkObjectResult(result.Data);
        }
        
        return new ObjectResult(new ProblemDetails()
        {
            Title = "An error occured",
            Status = 400,
            Detail = result.Error!.ErrorMessage,
        });
    }
    
}