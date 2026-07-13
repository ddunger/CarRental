using CarRental.Application.Common.Responses;
using CarRental.Domain.Enums;
using CarRental.Domain.Interfaces.Repositories;
using CarRental.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CarRental.Application.Reservations.Commands
{
    public record ConfirmReservationCommand(int ReservationId) : IRequest<ApplicationResult<StringResponse>>;

    public class ConfirmReservationCommandHandler(
        ILogger<ConfirmReservationCommandHandler> logger,
        IReservationsRepository reservationsRepository)
        : IRequestHandler<ConfirmReservationCommand, ApplicationResult<StringResponse>>
    {
        public async Task<ApplicationResult<StringResponse>> Handle(
            ConfirmReservationCommand command, CancellationToken cancellationToken)
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

            if (reservation.Status != ReservationStatus.Pending)
                return ApplicationResult<StringResponse>.Failure(
                    $"Only pending reservations can be confirmed. Current status: {reservation.Status}.",
                    ResultError.Conflict);

            reservation.Status = ReservationStatus.Confirmed;

            var updateResult = await reservationsRepository.UpdateReservationAsync(reservation, cancellationToken);
            if (!updateResult.IsSuccess)
                return ApplicationResult<StringResponse>.Failure(
                    "Failed to confirm reservation.", ResultError.Internal);

            logger.LogInformation("Reservation {ReservationId} confirmed.", reservation.Id);
            return ApplicationResult<StringResponse>.Success(new StringResponse("Reservation confirmed."));
        }
    }
}