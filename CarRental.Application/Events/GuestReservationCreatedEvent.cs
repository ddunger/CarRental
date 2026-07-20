using CarRental.Domain.Interfaces.Services;
using CarRental.Domain.Models.Mail;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CarRental.Application.Events
{
    public record GuestReservationCreatedEvent(
      string GuestEmail,
      string GuestFullName,
      int ReservationId,
      string VehicleName,
      string RegistrationPlate,
      DateTimeOffset PickupTimeUtc,
      DateTimeOffset ExpectedReturnTimeUtc,
      string PickupLocationName,
      decimal ExpectedCostEuro,
      string TrackingCode) : INotification;

    public class GuestReservationCreatedEventHandler(
        ILogger<GuestReservationCreatedEventHandler> logger,
        IConfiguration configuration,
        IMailService mailService) : INotificationHandler<GuestReservationCreatedEvent>
    {
        public async Task Handle(GuestReservationCreatedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                var baseUrl = configuration["App:FrontendBaseUrl"]?.TrimEnd('/') ?? "https://localhost:7063";
                var trackingUrl = $"{baseUrl}/track/{notification.TrackingCode}";

                var body = $"""
                    <h2>Your reservation is received</h2>
                    <p>Hello {notification.GuestFullName},</p>
                    <p>Thank you for your reservation. Here are the details:</p>
                    <ul>
                        <li><b>Reservation:</b> #{notification.ReservationId}</li>
                        <li><b>Vehicle:</b> {notification.VehicleName} ({notification.RegistrationPlate})</li>
                        <li><b>Pickup:</b> {notification.PickupTimeUtc:yyyy-MM-dd HH:mm} UTC, {notification.PickupLocationName}</li>
                        <li><b>Expected return:</b> {notification.ExpectedReturnTimeUtc:yyyy-MM-dd HH:mm} UTC</li>
                        <li><b>Estimated cost:</b> {notification.ExpectedCostEuro:F2} €</li>
                    </ul>
                    <p>
                        Track or cancel your reservation at any time using your personal link:<br />
                        <a href="{trackingUrl}">{trackingUrl}</a>
                    </p>
                    <p><b>Keep this email</b> — the link is the only way to access your reservation.</p>
                    """;

                var mailData = new MailDataModel
                {
                    EmailToId = notification.GuestEmail,
                    EmailToName = notification.GuestFullName,
                    EmailSubject = $"Reservation #{notification.ReservationId} — your tracking link",
                    EmailBody = body
                };

                await mailService.SendEmailAsync(mailData);
                logger.LogInformation("Tracking link email sent for reservation {ReservationId}.", notification.ReservationId);
            }
            catch (Exception ex)
            {
                // Email is best-effort: the guest already saw the link in the UI.
                logger.LogError(ex, "Failed to send tracking email for reservation {ReservationId}.", notification.ReservationId);
            }
        }
    }
}