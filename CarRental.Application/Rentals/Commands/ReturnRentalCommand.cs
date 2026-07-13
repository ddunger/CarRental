using CarRental.Application.Rentals.Requests;
using CarRental.Application.Rentals.Responses;
using CarRental.Domain.Enums;
using CarRental.Domain.Interfaces.Repositories;
using CarRental.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CarRental.Application.Rentals.Commands
{
    public record ReturnRentalCommand(int RentalId, ReturnRentalRequest Request)
        : IRequest<ApplicationResult<RentalResponse>>;

    public class ReturnRentalCommandHandler(
        ILogger<ReturnRentalCommandHandler> logger,
        IRentalsRepository rentalsRepository,
        IPickupLocationRepository locationRepository)
        : IRequestHandler<ReturnRentalCommand, ApplicationResult<RentalResponse>>
    {
        public async Task<ApplicationResult<RentalResponse>> Handle(
            ReturnRentalCommand command, CancellationToken cancellationToken)
        {
            var rentalResult = await rentalsRepository.GetRentalByIdAsync(command.RentalId, cancellationToken);
            if (!rentalResult.IsSuccess)
                return ApplicationResult<RentalResponse>.Failure(
                    "Failed to retrieve rental.", ResultError.Internal);

            var rental = rentalResult.Value;
            if (rental is null)
                return ApplicationResult<RentalResponse>.Failure(
                    "Rental not found.", ResultError.NotFound);

            if (rental.Status != RentalStatus.Active && rental.Status != RentalStatus.Overdue)
                return ApplicationResult<RentalResponse>.Failure(
                    $"Only active or overdue rentals can be returned. Current status: {rental.Status}.",
                    ResultError.Conflict);

            var returnTime = command.Request.ActualReturnTimeUtc ?? DateTimeOffset.UtcNow;
            if (returnTime < rental.ActualPickupTimeUtc)
                return ApplicationResult<RentalResponse>.Failure(
                    "Return time cannot be before pickup time.", ResultError.Validation);

            // Optional dropoff change
            if (command.Request.DropoffLocationId.HasValue &&
                command.Request.DropoffLocationId.Value != rental.DropoffLocationId)
            {
                var locationResult = await locationRepository.GetByIdAsync(
                    command.Request.DropoffLocationId.Value, cancellationToken);
                if (!locationResult.IsSuccess)
                    return ApplicationResult<RentalResponse>.Failure(
                        "Failed to validate dropoff location.", ResultError.Internal);
                if (locationResult.Value is null || !locationResult.Value.IsActive)
                    return ApplicationResult<RentalResponse>.Failure(
                        "Dropoff location not found.", ResultError.Validation);

                rental.DropoffLocationId = command.Request.DropoffLocationId.Value;
                rental.DropoffLocation = locationResult.Value;
            }

            // Recalculate final cost from actual duration (late returns cost more)
            var vehicleDailyPrice = rental.Vehicle?.PricePerDayInEuro;
            if (vehicleDailyPrice.HasValue)
            {
                var actualDays = (int)Math.Ceiling((returnTime - rental.ActualPickupTimeUtc).TotalDays);
                var actualCost = Math.Max(1, actualDays) * vehicleDailyPrice.Value;
                rental.TotalCostEuro = Math.Max(rental.TotalCostEuro, actualCost);
            }

            rental.ActualReturnTimeUtc = returnTime;
            rental.Status = RentalStatus.Completed;

            var updateResult = await rentalsRepository.UpdateRentalAsync(rental, cancellationToken);
            if (!updateResult.IsSuccess || updateResult.Value is null)
                return ApplicationResult<RentalResponse>.Failure(
                    "Failed to complete the return.", ResultError.Internal);

            logger.LogInformation("Rental {RentalId} returned. Final cost: {Cost} EUR.",
                rental.Id, rental.TotalCostEuro);

            var updated = updateResult.Value;
            return ApplicationResult<RentalResponse>.Success(new RentalResponse(
                updated.Id,
                updated.ReservationId,
                updated.CustomerId,
                updated.Customer?.Email ?? string.Empty,
                $"{updated.Customer?.FirstName} {updated.Customer?.LastName}".Trim(),
                updated.VehicleId,
                $"{updated.Vehicle?.Manufacturer?.Name} {updated.Vehicle?.VehicleModel}".Trim(),
                updated.Vehicle?.RegistrationPlate ?? string.Empty,
                updated.PickupLocationId,
                updated.PickupLocation?.Name ?? string.Empty,
                updated.DropoffLocationId,
                updated.DropoffLocation?.Name ?? string.Empty,
                updated.ActualPickupTimeUtc,
                updated.ExpectedReturnTimeUtc,
                updated.ActualReturnTimeUtc,
                updated.TotalCostEuro,
                updated.Status));
        }
    }
}