using CarRental.Application.SignalR;
using CarRental.Domain.Enums;
using CarRental.Domain.Interfaces.Repositories;

namespace CarRental.API.Hubs
{
    public class OverdueRentalsBackgroundService(
        ILogger<OverdueRentalsBackgroundService> logger,
        IServiceScopeFactory scopeFactory) : BackgroundService
    {
        private static readonly TimeSpan Interval = TimeSpan.FromMinutes(10);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Overdue rentals background service started.");

            using var timer = new PeriodicTimer(Interval);

            // Run once at startup, then on every tick
            do
            {
                try
                {
                    await ProcessOverdueRentalsAsync(stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error while processing overdue rentals.");
                }
            }
            while (await timer.WaitForNextTickAsync(stoppingToken));

            logger.LogInformation("Overdue rentals background service stopped.");
        }

        private async Task ProcessOverdueRentalsAsync(CancellationToken cancellationToken)
        {
            // BackgroundService is a singleton; repositories are scoped — create a scope per tick
            using var scope = scopeFactory.CreateScope();
            var rentalsRepository = scope.ServiceProvider.GetRequiredService<IRentalsRepository>();
            var notificationPublisher = scope.ServiceProvider.GetRequiredService<IStaffNotificationPublisher>();

            var now = DateTimeOffset.UtcNow;
            var overdueResult = await rentalsRepository.GetOverdueRentalsAsync(now, cancellationToken);

            if (!overdueResult.IsSuccess || overdueResult.Value is null)
            {
                logger.LogWarning("Failed to fetch overdue rentals.");
                return;
            }

            foreach (var rental in overdueResult.Value)
            {
                rental.Status = RentalStatus.Overdue;
                var updateResult = await rentalsRepository.UpdateRentalAsync(rental, cancellationToken);

                if (!updateResult.IsSuccess)
                {
                    logger.LogError("Failed to mark rental {RentalId} as overdue.", rental.Id);
                    continue;
                }

                logger.LogInformation("Rental {RentalId} marked as overdue (expected return {ExpectedReturn}).",
                    rental.Id, rental.ExpectedReturnTimeUtc);

                var notification = new RentalOverdueNotification(
                    rental.Id,
                    rental.Customer?.Email ?? string.Empty,
                    $"{rental.Customer?.FirstName} {rental.Customer?.LastName}".Trim(),
                    $"{rental.Vehicle?.Manufacturer?.Name} {rental.Vehicle?.VehicleModel}".Trim(),
                    rental.Vehicle?.RegistrationPlate ?? string.Empty,
                    rental.ExpectedReturnTimeUtc,
                    Math.Round((now - rental.ExpectedReturnTimeUtc).TotalHours, 1));

                try
                {
                    await notificationPublisher.PublishRentalOverdueAsync(notification, cancellationToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to publish overdue notification for rental {RentalId}.", rental.Id);
                }
            }
        }
    }
}