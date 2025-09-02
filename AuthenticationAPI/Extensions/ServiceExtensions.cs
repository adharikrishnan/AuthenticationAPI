using System.Text;
using System.Text.Json;
using AuthenticationAPI.DataAccess;
using AuthenticationAPI.Models.Configurations;
using AuthenticationAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        services.AddSingleton(configuration.GetSection("Authentication")
                .Get<AuthConfiguration>() ??
                throw new NullReferenceException("Failed to register Authentication Configuration as the section could not be found."));
    }

    public static void SetupServices(this IServiceCollection services)
    {
        services.AddSingleton<ITokenHelper, TokenHelper>();
        services.AddSingleton<IPasswordHelper, PasswordHelper>();
        services.AddScoped<IAuthService, AuthService>();
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
        var authConfiguration = services.BuildServiceProvider().GetRequiredService<AuthConfiguration>();

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
                OnAuthenticationFailed = context =>
                {
                    var problem = new ProblemDetails
                    {
                        Title = "Authentication Failed",
                        Status = StatusCodes.Status401Unauthorized,
                        Detail = context.Exception.Message,
                        Instance = context.HttpContext.Request.Path,
                    };
                    
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";
                    var json = JsonSerializer.Serialize(problem);
                    return context.Response.WriteAsync(json);
                }
            };
        });

        services.AddAuthorization();
    }

}
