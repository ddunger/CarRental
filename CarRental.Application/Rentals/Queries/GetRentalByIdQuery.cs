using CarRental.Application.Rentals.Responses;
using CarRental.Domain.Constants;
using CarRental.Domain.Enums;
using CarRental.Domain.Interfaces.Application;
using CarRental.Domain.Interfaces.Repositories;
using CarRental.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CarRental.Application.Rentals.Queries
{
    public record GetRentalByIdQuery(int RentalId) : IRequest<ApplicationResult<RentalResponse>>;

    public class GetRentalByIdQueryHandler(
        ILogger<GetRentalByIdQueryHandler> logger,
        IUserContext userContext,
        IRentalsRepository rentalsRepository)
        : IRequestHandler<GetRentalByIdQuery, ApplicationResult<RentalResponse>>
    {
        public async Task<ApplicationResult<RentalResponse>> Handle(
            GetRentalByIdQuery query, CancellationToken cancellationToken)
        {
            var currentUser = userContext.GetCurrentUser();
            if (currentUser is null)
                return ApplicationResult<RentalResponse>.Failure("Unauthorized", ResultError.Unauthorized);

            var rentalResult = await rentalsRepository.GetRentalByIdAsync(query.RentalId, cancellationToken);
            if (!rentalResult.IsSuccess)
                return ApplicationResult<RentalResponse>.Failure(
                    "Failed to retrieve rental.", ResultError.Internal);

            var rental = rentalResult.Value;

            var isStaff = currentUser.IsInRole(Roles.Admin) || currentUser.IsInRole(Roles.Manager);
            if (rental is null || (!isStaff && rental.CustomerId != currentUser.Id))
            {
                logger.LogInformation("Rental {RentalId} not found or not accessible.", query.RentalId);
                return ApplicationResult<RentalResponse>.Failure("Rental not found.", ResultError.NotFound);
            }

            return ApplicationResult<RentalResponse>.Success(new RentalResponse(
                rental.Id,
                rental.ReservationId,
                rental.CustomerId,
                rental.Customer?.Email ?? string.Empty,
                $"{rental.Customer?.FirstName} {rental.Customer?.LastName}".Trim(),
                rental.VehicleId,
                $"{rental.Vehicle?.Manufacturer?.Name} {rental.Vehicle?.VehicleModel}".Trim(),
                rental.Vehicle?.RegistrationPlate ?? string.Empty,
                rental.PickupLocationId,
                rental.PickupLocation?.Name ?? string.Empty,
                rental.DropoffLocationId,
                rental.DropoffLocation?.Name ?? string.Empty,
                rental.ActualPickupTimeUtc,
                rental.ExpectedReturnTimeUtc,
                rental.ActualReturnTimeUtc,
                rental.TotalCostEuro,
                rental.Status));
        }
    }
}