using CarRental.Application.Reservations.Responses;
using CarRental.Domain.Constants;
using CarRental.Domain.Enums;
using CarRental.Domain.Interfaces.Application;
using CarRental.Domain.Interfaces.Repositories;
using CarRental.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CarRental.Application.Reservations.Queries
{
    public record GetReservationByIdQuery(int ReservationId) : IRequest<ApplicationResult<ReservationResponse>>;

    public class GetReservationByIdQueryHandler(
        ILogger<GetReservationByIdQueryHandler> logger,
        IUserContext userContext,
        IReservationsRepository reservationsRepository)
        : IRequestHandler<GetReservationByIdQuery, ApplicationResult<ReservationResponse>>
    {
        public async Task<ApplicationResult<ReservationResponse>> Handle(
            GetReservationByIdQuery query, CancellationToken cancellationToken)
        {
            var currentUser = userContext.GetCurrentUser();
            if (currentUser is null)
                return ApplicationResult<ReservationResponse>.Failure(
                    "Unauthorized", ResultError.Unauthorized);

            var reservationResult = await reservationsRepository.GetReservationByIdAsync(
                query.ReservationId, cancellationToken);

            if (!reservationResult.IsSuccess)
                return ApplicationResult<ReservationResponse>.Failure(
                    "Failed to retrieve reservation.", ResultError.Internal);

            var reservation = reservationResult.Value;

            var isStaff = currentUser.IsInRole(Roles.Admin) || currentUser.IsInRole(Roles.Manager);
            if (reservation is null || (!isStaff && reservation.CustomerId != currentUser.Id))
            {
                logger.LogInformation("Reservation {ReservationId} not found or not accessible.", query.ReservationId);
                return ApplicationResult<ReservationResponse>.Failure(
                    "Reservation not found.", ResultError.NotFound);
            }

            return ApplicationResult<ReservationResponse>.Success(new ReservationResponse(
                reservation.Id,
                reservation.CustomerId,
                reservation.Customer?.Email ?? string.Empty,
                $"{reservation.Customer?.FirstName} {reservation.Customer?.LastName}".Trim(),
                reservation.VehicleId,
                $"{reservation.Vehicle?.Manufacturer?.Name} {reservation.Vehicle?.VehicleModel}".Trim(),
                reservation.Vehicle?.RegistrationPlate ?? string.Empty,
                reservation.PickupLocationId,
                reservation.PickupLocation?.Name ?? string.Empty,
                reservation.DropoffLocationId,
                reservation.DropoffLocation?.Name ?? string.Empty,
                reservation.PickupTimeUtc,
                reservation.ExpectedReturnTimeUtc,
                reservation.ExpectedCostEuro,
                reservation.Status,
                reservation.CreatedAtUtc,
                reservation.CancelledAtUtc));
        }
    }
}