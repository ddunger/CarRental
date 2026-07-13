using CarRental.Application.Rentals.Responses;
using CarRental.Domain.Entities;
using CarRental.Domain.Enums;
using CarRental.Domain.Interfaces.Application;
using CarRental.Domain.Interfaces.Repositories;
using CarRental.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CarRental.Application.Rentals.Queries
{
    public record GetMyRentalsQuery(int? Offset, int? Limit)
        : IRequest<ApplicationResult<IEnumerable<RentalResponse>>>;

    public class GetMyRentalsQueryHandler(
        ILogger<GetMyRentalsQueryHandler> logger,
        IUserContext userContext,
        IRentalsRepository rentalsRepository)
        : IRequestHandler<GetMyRentalsQuery, ApplicationResult<IEnumerable<RentalResponse>>>
    {
        public async Task<ApplicationResult<IEnumerable<RentalResponse>>> Handle(
            GetMyRentalsQuery query, CancellationToken cancellationToken)
        {
            var currentUser = userContext.GetCurrentUser();
            if (currentUser is null)
                return ApplicationResult<IEnumerable<RentalResponse>>.Failure(
                    "Unauthorized", ResultError.Unauthorized);

            if (query.Offset.HasValue && query.Offset < 0)
                return ApplicationResult<IEnumerable<RentalResponse>>.Failure(
                    "Offset must be zero or greater.", ResultError.Validation);

            if (query.Limit.HasValue && query.Limit < 1)
                return ApplicationResult<IEnumerable<RentalResponse>>.Failure(
                    "Limit must be 1 or greater.", ResultError.Validation);

            var rentalsResult = await rentalsRepository.GetAllRentalsAsync(
                customerId: currentUser.Id,
                vehicleId: null,
                pickupLocationId: null,
                status: null,
                pickupFromUtc: null,
                pickupToUtc: null,
                query.Offset,
                query.Limit,
                cancellationToken);

            if (!rentalsResult.IsSuccess)
                return ApplicationResult<IEnumerable<RentalResponse>>.Failure(
                    "Failed to retrieve rentals.", ResultError.Internal);

            var rentals = (rentalsResult.Value ?? Enumerable.Empty<RentalEntity>()).ToList();
            logger.LogInformation("Fetched {Count} rental(s) for user {UserId}.", rentals.Count, currentUser.Id);

            return ApplicationResult<IEnumerable<RentalResponse>>.Success(
                rentals.Select(r => new RentalResponse(
                    r.Id,
                    r.ReservationId,
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
                    r.ActualPickupTimeUtc,
                    r.ExpectedReturnTimeUtc,
                    r.ActualReturnTimeUtc,
                    r.TotalCostEuro,
                    r.Status)));
        }
    }
}