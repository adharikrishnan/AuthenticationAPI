using System.Text;
using System.Text.Json;
using AuthenticationAPI.DataAccess;
using AuthenticationAPI.Models.Configurations;
using AuthenticationAPI.Services;
using AuthenticationAPI.Validators;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace AuthenticationAPI.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AuthDbContext>(opts =>
        {
            opts.UseSqlite(configuration["Database:Sqlite:ConnectionString"], b =>
            b.MigrationsAssembly("AuthenticationAPI"));
            opts.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });
    }

    public static void SetupConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AuthConfiguration>(configuration.GetSection("Authentication"));
    }

    public static void SetupServices(this IServiceCollection services)
    {
        services.AddScoped<IValidatorFactory, ValidatorFactory>();
        services.AddSingleton<ITokenHelper, TokenHelper>();
        services.AddSingleton<IPasswordHelper, PasswordHelper>();
        services.AddScoped<IAuthService, AuthService>();
    }

    public static void SetupBackgroundServices(this IServiceCollection services)
    {
        services.AddHostedService<RefreshTokenCleanupService>();
    }

    public static void SetupSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "AuthorizationAPI", Version = "v1" });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme. Enter you token here",
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    []
                }
            });

        });
    }

    public static void ConfigureAuthentication(this IServiceCollection services)
    {
        var authConfiguration = services.BuildServiceProvider()
            .GetRequiredService<IOptions<AuthConfiguration>>().Value;

        services.AddAuthentication().AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = authConfiguration.Issuer,
                ValidAudience = authConfiguration.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authConfiguration.SecretKey))
            };

            options.Events = new JwtBearerEvents()
            {
                OnMessageReceived = context =>
                {
                    // Checking to see if this is an authorized endpoint.
                    // If it is not authorized endpoint, we won't check if the token is present in the header.
                    var isNotAuthorizedEndpont = context.HttpContext.GetEndpoint()?.Metadata?.GetMetadata<IAuthorizeData>() is null;

                    if (isNotAuthorizedEndpont)
                    {
                        return Task.CompletedTask;
                    }

                    var token = context.Request.Headers["Authorization"].FirstOrDefault();

                    if (!string.IsNullOrEmpty(token))
                        return Task.CompletedTask;

                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";

                    return context.Response.WriteAsync(
                        CreateProblemDetailsString
                        ("Authentication Failed",
                        "Authentication Token has not been provided.",
                        StatusCodes.Status401Unauthorized));

                },
                OnAuthenticationFailed = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";

                    return context.Response.WriteAsync(
                        CreateProblemDetailsString
                        ("Authentication Failed",
                        "Invalid Authentication Token.",
                        StatusCodes.Status401Unauthorized));
                },
                OnForbidden = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    context.Response.ContentType = "application/json";
                    return context.Response.WriteAsync(
                       CreateProblemDetailsString
                       ("Authentication Failed",
                       "You are not authorized to access this resource.",
                       StatusCodes.Status403Forbidden));
                }
            };
        });

        services.AddAuthorization();
    }

    private static string CreateProblemDetailsString(string title, string detail, int statusCode)
    {
        var problem = new ProblemDetails
        {
            Title = title,
            Status = statusCode,
            Detail = detail,
        };
        return JsonSerializer.Serialize(problem);
    }

}
