using Serilog;

namespace AuthenticationAPI.Extensions;

public static class BuilderExtensions
{
    public static void SetupSerilog(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, configuration) =>
        {
            configuration
                .ReadFrom.Configuration(context.Configuration);
        });


    }

}
