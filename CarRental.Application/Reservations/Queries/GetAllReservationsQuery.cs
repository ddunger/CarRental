using CarRental.Application.Reservations.Requests;
using CarRental.Application.Reservations.Responses;
using CarRental.Domain.Entities;
using CarRental.Domain.Enums;
using CarRental.Domain.Interfaces.Application;
using CarRental.Domain.Interfaces.Repositories;
using CarRental.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CarRental.Application.Reservations.Queries
{
    public record GetAllReservationsQuery(GetReservationsRequest Request, int? Offset, int? Limit)
        : IRequest<ApplicationResult<IEnumerable<ReservationResponse>>>;

    public class GetAllReservationsQueryHandler(
        ILogger<GetAllReservationsQueryHandler> logger,
        IUserContext userContext,
        IReservationsRepository reservationsRepository)
        : IRequestHandler<GetAllReservationsQuery, ApplicationResult<IEnumerable<ReservationResponse>>>
    {
        public async Task<ApplicationResult<IEnumerable<ReservationResponse>>> Handle(
            GetAllReservationsQuery query, CancellationToken cancellationToken)
        {
            var currentUser = userContext.GetCurrentUser();
            if (currentUser is null)
                return ApplicationResult<IEnumerable<ReservationResponse>>.Failure(
                    "Unauthorized", ResultError.Unauthorized);

            if (query.Offset.HasValue && query.Offset < 0)
                return ApplicationResult<IEnumerable<ReservationResponse>>.Failure(
                    "Offset must be zero or greater.", ResultError.Validation);

            if (query.Limit.HasValue && query.Limit < 1)
                return ApplicationResult<IEnumerable<ReservationResponse>>.Failure(
                    "Limit must be 1 or greater.", ResultError.Validation);

            // Customers are always scoped to their own reservations
            var isStaff = currentUser.IsInRole("Admin") || currentUser.IsInRole("Manager");
            var customerId = isStaff ? query.Request.CustomerId : currentUser.Id;

            var reservationsResult = await reservationsRepository.GetAllReservationsAsync(
                customerId,
                query.Request.VehicleId,
                query.Request.PickupLocationId,
                query.Request.Status,
                query.Request.PickupFromUtc,
                query.Request.PickupToUtc,
                query.Offset,
                query.Limit,
                cancellationToken);

            if (!reservationsResult.IsSuccess)
                return ApplicationResult<IEnumerable<ReservationResponse>>.Failure(
                    "Failed to retrieve reservations.", ResultError.Internal);

            var reservations = (reservationsResult.Value ?? Enumerable.Empty<ReservationEntity>()).ToList();
            logger.LogInformation("Fetched {Count} reservation(s).", reservations.Count);

            return ApplicationResult<IEnumerable<ReservationResponse>>.Success(
                reservations.Select(r => new ReservationResponse(
                    r.Id,
                    r.CustomerId,
                    r.Customer?.Email ?? string.Empty,
                    $"{r.Customer?.FirstName} {r.Customer?.LastName}".Trim(),
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
                    r.CancelledAtUtc)));
        }
    }
}