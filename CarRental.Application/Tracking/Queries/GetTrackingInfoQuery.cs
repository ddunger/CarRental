using CarRental.Application.Rentals.Responses;
using CarRental.Application.Reservations.Responses;
using CarRental.Application.Tracking.Responses;
using CarRental.Domain.Entities;
using CarRental.Domain.Enums;
using CarRental.Domain.Interfaces.Repositories;
using CarRental.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CarRental.Application.Tracking.Queries
{
    public record GetTrackingInfoQuery(string TrackingCode) : IRequest<ApplicationResult<TrackingResponse>>;

    public class GetTrackingInfoQueryHandler(
        ILogger<GetTrackingInfoQueryHandler> logger,
        IReservationsRepository reservationsRepository,
        IRentalsRepository rentalsRepository)
        : IRequestHandler<GetTrackingInfoQuery, ApplicationResult<TrackingResponse>>
    {
        public async Task<ApplicationResult<TrackingResponse>> Handle(
            GetTrackingInfoQuery query, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(query.TrackingCode) || query.TrackingCode.Length > 64)
                return ApplicationResult<TrackingResponse>.Failure("Not found.", ResultError.NotFound);

            // Try as a reservation code first
            var reservationResult = await reservationsRepository.GetReservationByTrackingCodeAsync(
                query.TrackingCode, cancellationToken);
            if (!reservationResult.IsSuccess)
                return ApplicationResult<TrackingResponse>.Failure("Failed to retrieve tracking info.", ResultError.Internal);

            if (reservationResult.Value is not null)
            {
                var reservation = reservationResult.Value;

                RentalResponse? linkedRental = null;
                if (reservation.Status == ReservationStatus.Converted)
                {
                    var rentalResult = await rentalsRepository.GetRentalByReservationIdAsync(
                        reservation.Id, cancellationToken);
                    if (rentalResult.IsSuccess && rentalResult.Value is not null)
                        linkedRental = MapRental(rentalResult.Value);
                }

                return ApplicationResult<TrackingResponse>.Success(new TrackingResponse(
                    MapReservation(reservation), linkedRental));
            }

            // Not a reservation code — try as a rental code (walk-in rentals)
            var directRentalResult = await rentalsRepository.GetRentalByTrackingCodeAsync(
                query.TrackingCode, cancellationToken);
            if (!directRentalResult.IsSuccess)
                return ApplicationResult<TrackingResponse>.Failure("Failed to retrieve tracking info.", ResultError.Internal);

            if (directRentalResult.Value is not null)
                return ApplicationResult<TrackingResponse>.Success(new TrackingResponse(
                    null, MapRental(directRentalResult.Value)));

            logger.LogInformation("Tracking lookup failed: unknown code.");
            return ApplicationResult<TrackingResponse>.Failure("Not found.", ResultError.NotFound);
        }

        private static ReservationResponse MapReservation(ReservationEntity r) => new(
            r.Id,
            r.CustomerId,
            r.CustomerId is not null ? r.Customer?.Email ?? string.Empty : r.GuestEmail ?? string.Empty,
            r.CustomerId is not null
                ? $"{r.Customer?.FirstName} {r.Customer?.LastName}".Trim()
                : r.GuestFullName ?? string.Empty,
            r.CustomerId is null,
            r.TrackingCode,
            r.VehicleId,
            $"{r.Vehicle?.Manufacturer?.Name} {r.Vehicle?.VehicleModel}".Trim(),
            r.Vehicle?.RegistrationPlate ?? string.Empty,
            r.PickupLocationId,
            r.PickupLocation?.Name ?? string.Empty,
            r.DropoffLocationId,
            r.DropoffLocation?.Name ?? string.Empty,
            r.PickupTimeUtc,
            r.ExpectedReturnTimeUtc,
            r.ExpectedCostEuro,
            r.Status,
            r.CreatedAtUtc,
            r.CancelledAtUtc);

        private static RentalResponse MapRental(RentalEntity r) => new(
            r.Id,
            r.ReservationId,
            r.CustomerId,
            r.CustomerId is not null ? r.Customer?.Email ?? string.Empty : r.GuestEmail ?? string.Empty,
            r.CustomerId is not null
                ? $"{r.Customer?.FirstName} {r.Customer?.LastName}".Trim()
                : r.GuestFullName ?? string.Empty,
            r.CustomerId is null,
            r.TrackingCode,
            r.VehicleId,
            $"{r.Vehicle?.Manufacturer?.Name} {r.Vehicle?.VehicleModel}".Trim(),
            r.Vehicle?.RegistrationPlate ?? string.Empty,
            r.PickupLocationId,
            r.PickupLocation?.Name ?? string.Empty,
            r.DropoffLocationId,
            r.DropoffLocation?.Name ?? string.Empty,
            r.ActualPickupTimeUtc,
            r.ExpectedReturnTimeUtc,
            r.ActualReturnTimeUtc,
            r.TotalCostEuro,
            r.Status);
    }
}