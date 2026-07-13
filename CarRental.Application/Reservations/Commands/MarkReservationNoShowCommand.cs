using CarRental.Application.Common.Responses;
using CarRental.Domain.Enums;
using CarRental.Domain.Interfaces.Repositories;
using CarRental.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CarRental.Application.Reservations.Commands
{
    public record MarkReservationNoShowCommand(int ReservationId) : IRequest<ApplicationResult<StringResponse>>;

    public class MarkReservationNoShowCommandHandler(
        ILogger<MarkReservationNoShowCommandHandler> logger,
        IReservationsRepository reservationsRepository)
        : IRequestHandler<MarkReservationNoShowCommand, ApplicationResult<StringResponse>>
    {
        public async Task<ApplicationResult<StringResponse>> Handle(
            MarkReservationNoShowCommand command, CancellationToken cancellationToken)
        {
            var reservationResult = await reservationsRepository.GetReservationByIdAsync(
                command.ReservationId, cancellationToken);

            if (!reservationResult.IsSuccess)
                return ApplicationResult<StringResponse>.Failure(
                    "Failed to retrieve reservation.", ResultError.Internal);

            var reservation = reservationResult.Value;
            if (reservation is null)
                return ApplicationResult<StringResponse>.Failure(
                    "Reservation not found.", ResultError.NotFound);

            if (reservation.Status != ReservationStatus.Confirmed &&
                reservation.Status != ReservationStatus.Pending)
                return ApplicationResult<StringResponse>.Failure(
                    $"Only pending or confirmed reservations can be marked as no-show. Current status: {reservation.Status}.",
                    ResultError.Conflict);

            if (reservation.PickupTimeUtc > DateTimeOffset.UtcNow)
                return ApplicationResult<StringResponse>.Failure(
                    "Cannot mark as no-show before the scheduled pickup time.", ResultError.Conflict);

            reservation.Status = ReservationStatus.NoShow;

            var updateResult = await reservationsRepository.UpdateReservationAsync(reservation, cancellationToken);
            if (!updateResult.IsSuccess)
                return ApplicationResult<StringResponse>.Failure(
                    "Failed to update reservation.", ResultError.Internal);

            logger.LogInformation("Reservation {ReservationId} marked as no-show.", reservation.Id);
            return ApplicationResult<StringResponse>.Success(new StringResponse("Reservation marked as no-show."));
        }
    }
}