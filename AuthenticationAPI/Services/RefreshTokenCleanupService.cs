namespace AuthenticationAPI.Services;

public class RefreshTokenCleanupService(ILogger<RefreshTokenCleanupService> logger, IServiceProvider serviceProvider) : BackgroundService
{

    private readonly PeriodicTimer _timer = new PeriodicTimer(TimeSpan.FromMinutes(15));
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await _timer.WaitForNextTickAsync(stoppingToken)
              && !stoppingToken.IsCancellationRequested)
        {
            await HandleCleanupAsync(stoppingToken);
        }

        _timer.Dispose();
    }

    private async Task HandleCleanupAsync(CancellationToken stoppingToken)
    {
        using var scope = serviceProvider.CreateScope();
        var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
        logger.LogInformation("Refresh Token Cleanup Service is running started at {time}" , DateTime.UtcNow);
        int recordsDeleted = await authService.DeleteInvalidRefreshTokensAsync(stoppingToken);
        logger.LogInformation("Refresh Token Cleanup Service is completed. Deleted {count} records by {time}" , recordsDeleted, DateTime.UtcNow);
    }

    
}
