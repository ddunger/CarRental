using CarRental.Application.Common.Responses;
using CarRental.Domain.Enums;
using CarRental.Domain.Interfaces.Application;
using CarRental.Domain.Interfaces.Repositories;
using CarRental.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CarRental.Application.Reservations.Commands
{
    public record CancelReservationCommand(int ReservationId) : IRequest<ApplicationResult<StringResponse>>;

    public class CancelReservationCommandHandler(
        ILogger<CancelReservationCommandHandler> logger,
        IUserContext userContext,
        IReservationsRepository reservationsRepository)
        : IRequestHandler<CancelReservationCommand, ApplicationResult<StringResponse>>
    {
        public async Task<ApplicationResult<StringResponse>> Handle(
            CancelReservationCommand command, CancellationToken cancellationToken)
        {
            var currentUser = userContext.GetCurrentUser();
            if (currentUser is null)
                return ApplicationResult<StringResponse>.Failure("Unauthorized", ResultError.Unauthorized);

            var reservationResult = await reservationsRepository.GetReservationByIdAsync(
                command.ReservationId, cancellationToken);

            if (!reservationResult.IsSuccess)
                return ApplicationResult<StringResponse>.Failure(
                    "Failed to retrieve reservation.", ResultError.Internal);

            var reservation = reservationResult.Value;

            var isStaff = currentUser.IsInRole("Admin") || currentUser.IsInRole("Manager");
            if (reservation is null || (!isStaff && reservation.CustomerId != currentUser.Id))
                return ApplicationResult<StringResponse>.Failure(
                    "Reservation not found.", ResultError.NotFound);

            if (reservation.Status != ReservationStatus.Pending &&
                reservation.Status != ReservationStatus.Confirmed)
                return ApplicationResult<StringResponse>.Failure(
                    $"Only pending or confirmed reservations can be cancelled. Current status: {reservation.Status}.",
                    ResultError.Conflict);

            reservation.Status = ReservationStatus.Cancelled;
            reservation.CancelledAtUtc = DateTimeOffset.UtcNow;

            var updateResult = await reservationsRepository.UpdateReservationAsync(reservation, cancellationToken);
            if (!updateResult.IsSuccess)
                return ApplicationResult<StringResponse>.Failure(
                    "Failed to cancel reservation.", ResultError.Internal);

            logger.LogInformation("Reservation {ReservationId} cancelled by user {UserId}.",
                reservation.Id, currentUser.Id);
            return ApplicationResult<StringResponse>.Success(new StringResponse("Reservation cancelled."));
        }
    }
}