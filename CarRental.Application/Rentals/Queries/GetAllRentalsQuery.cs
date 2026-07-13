using CarRental.Application.Rentals.Requests;
using CarRental.Application.Rentals.Responses;
using CarRental.Domain.Constants;
using CarRental.Domain.Entities;
using CarRental.Domain.Enums;
using CarRental.Domain.Interfaces.Application;
using CarRental.Domain.Interfaces.Repositories;
using CarRental.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CarRental.Application.Rentals.Queries
{
    public record GetAllRentalsQuery(GetRentalsRequest Request, int? Offset, int? Limit)
        : IRequest<ApplicationResult<IEnumerable<RentalResponse>>>;

    public class GetAllRentalsQueryHandler(
        ILogger<GetAllRentalsQueryHandler> logger,
        IUserContext userContext,
        IRentalsRepository rentalsRepository)
        : IRequestHandler<GetAllRentalsQuery, ApplicationResult<IEnumerable<RentalResponse>>>
    {
        public async Task<ApplicationResult<IEnumerable<RentalResponse>>> Handle(
            GetAllRentalsQuery query, CancellationToken cancellationToken)
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

            var isStaff = currentUser.IsInRole(Roles.Admin) || currentUser.IsInRole(Roles.Manager);
            var customerId = isStaff ? query.Request.CustomerId : currentUser.Id;

            var rentalsResult = await rentalsRepository.GetAllRentalsAsync(
                customerId,
                query.Request.VehicleId,
                query.Request.PickupLocationId,
                query.Request.Status,
                query.Request.PickupFromUtc,
                query.Request.PickupToUtc,
                query.Offset,
                query.Limit,
                cancellationToken);

            if (!rentalsResult.IsSuccess)
                return ApplicationResult<IEnumerable<RentalResponse>>.Failure(
                    "Failed to retrieve rentals.", ResultError.Internal);

            var rentals = (rentalsResult.Value ?? Enumerable.Empty<RentalEntity>()).ToList();
            logger.LogInformation("Fetched {Count} rental(s).", rentals.Count);

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