using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationAPI.Exceptions;

internal sealed class GlobalExceptionHandler(RequestDelegate next, ILogger<GlobalExceptionHandler> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occured. TraceId: {TraceId}", context.TraceIdentifier);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";

            var problem = new ProblemDetails
            {
                Title = "An unexpected error occured.",
                Status = StatusCodes.Status500InternalServerError,
                Detail = "An unexpected error occured. Please contact support with this Id" + context.TraceIdentifier,
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(problem)).ConfigureAwait(false);
        }
    }

}
