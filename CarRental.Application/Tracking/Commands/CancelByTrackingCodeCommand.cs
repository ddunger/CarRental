using CarRental.Application.Common.Responses;
using CarRental.Domain.Enums;
using CarRental.Domain.Interfaces.Repositories;
using CarRental.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CarRental.Application.Tracking.Commands
{
    public record CancelByTrackingCodeCommand(string TrackingCode) : IRequest<ApplicationResult<StringResponse>>;

    public class CancelByTrackingCodeCommandHandler(
        ILogger<CancelByTrackingCodeCommandHandler> logger,
        IReservationsRepository reservationsRepository)
        : IRequestHandler<CancelByTrackingCodeCommand, ApplicationResult<StringResponse>>
    {
        public async Task<ApplicationResult<StringResponse>> Handle(
            CancelByTrackingCodeCommand command, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(command.TrackingCode) || command.TrackingCode.Length > 64)
                return ApplicationResult<StringResponse>.Failure("Not found.", ResultError.NotFound);

            var reservationResult = await reservationsRepository.GetReservationByTrackingCodeAsync(
                command.TrackingCode, cancellationToken);
            if (!reservationResult.IsSuccess)
                return ApplicationResult<StringResponse>.Failure("Failed to retrieve reservation.", ResultError.Internal);

            var reservation = reservationResult.Value;
            if (reservation is null)
                return ApplicationResult<StringResponse>.Failure("Not found.", ResultError.NotFound);

            if (reservation.Status != ReservationStatus.Pending &&
                reservation.Status != ReservationStatus.Confirmed)
                return ApplicationResult<StringResponse>.Failure(
                    $"Only pending or confirmed reservations can be cancelled. Current status: {reservation.Status}.",
                    ResultError.Conflict);

            reservation.Status = ReservationStatus.Cancelled;
            reservation.CancelledAtUtc = DateTimeOffset.UtcNow;

            var updateResult = await reservationsRepository.UpdateReservationAsync(reservation, cancellationToken);
            if (!updateResult.IsSuccess)
                return ApplicationResult<StringResponse>.Failure("Failed to cancel reservation.", ResultError.Internal);

            logger.LogInformation("Reservation {ReservationId} cancelled via tracking code.", reservation.Id);
            return ApplicationResult<StringResponse>.Success(new StringResponse("Reservation cancelled."));
        }
    }
}