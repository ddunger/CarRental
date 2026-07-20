using CarRental.Application.Rentals.Requests;
using CarRental.Application.Rentals.Responses;
using CarRental.Domain.Entities;
using CarRental.Domain.Enums;
using CarRental.Domain.Helpers;
using CarRental.Domain.Interfaces.Repositories;
using CarRental.Domain.Results; 
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CarRental.Application.Rentals.Commands
{
    public record CreateRentalCommand(CreateRentalRequest Request)
        : IRequest<ApplicationResult<RentalResponse>>;

    public class CreateRentalCommandHandler(
        ILogger<CreateRentalCommandHandler> logger,
        IRentalsRepository rentalsRepository,
        IReservationsRepository reservationsRepository,
        IVehiclesRepository vehiclesRepository,
        IPickupLocationRepository locationRepository,
        UserManager<UserEntity> userManager)
        : IRequestHandler<CreateRentalCommand, ApplicationResult<RentalResponse>>
    {
        public async Task<ApplicationResult<RentalResponse>> Handle(
            CreateRentalCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request;

            return request.ReservationId.HasValue
                ? await HandleFromReservationAsync(request.ReservationId.Value, cancellationToken)
                : await HandleWalkInAsync(request, cancellationToken);
        }

        // --- Flow A: convert a reservation ---
        private async Task<ApplicationResult<RentalResponse>> HandleFromReservationAsync(
            int reservationId, CancellationToken cancellationToken)
        {
            var reservationResult = await reservationsRepository.GetReservationByIdAsync(reservationId, cancellationToken);
            if (!reservationResult.IsSuccess)
                return ApplicationResult<RentalResponse>.Failure(
                    "Failed to retrieve reservation.", ResultError.Internal);

            var reservation = reservationResult.Value;
            if (reservation is null)
                return ApplicationResult<RentalResponse>.Failure(
                    "Reservation not found.", ResultError.NotFound);

            if (reservation.Status != ReservationStatus.Confirmed)
                return ApplicationResult<RentalResponse>.Failure(
                    $"Only confirmed reservations can be converted to a rental. Current status: {reservation.Status}.",
                    ResultError.Conflict);

            var availabilityError = await CheckVehicleNotOutAsync(reservation.VehicleId, cancellationToken);
            if (availabilityError is not null)
                return availabilityError;

            var now = DateTimeOffset.UtcNow;
            var rental = new RentalEntity
            {
                ReservationId = reservation.Id,
                CustomerId = reservation.CustomerId,
                GuestEmail = reservation.GuestEmail,
                GuestFullName = reservation.GuestFullName,
                GuestPhone = reservation.GuestPhone,
                TrackingCode = TrackingCodeGenerator.Generate(),
                VehicleId = reservation.VehicleId,
                PickupLocationId = reservation.PickupLocationId,
                DropoffLocationId = reservation.DropoffLocationId,
                ActualPickupTimeUtc = now,
                ExpectedReturnTimeUtc = reservation.ExpectedReturnTimeUtc,
                TotalCostEuro = reservation.ExpectedCostEuro,
                Status = RentalStatus.Active
            };

            var createdResult = await rentalsRepository.AddRentalAsync(rental, cancellationToken);
            if (!createdResult.IsSuccess || createdResult.Value is null)
                return ApplicationResult<RentalResponse>.Failure(
                    "Failed to create rental.", ResultError.Internal);

            // Mark the reservation as converted
            reservation.Status = ReservationStatus.Converted;
            var reservationUpdate = await reservationsRepository.UpdateReservationAsync(reservation, cancellationToken);
            if (!reservationUpdate.IsSuccess)
                logger.LogError("Rental {RentalId} created but reservation {ReservationId} could not be marked Converted.",
                    createdResult.Value.Id, reservation.Id);

            logger.LogInformation("Rental {RentalId} created from reservation {ReservationId}.",
                createdResult.Value.Id, reservation.Id);

            return await BuildResponseAsync(createdResult.Value, cancellationToken);
        }

        // --- Flow B: walk-in ---
        private async Task<ApplicationResult<RentalResponse>> HandleWalkInAsync(
            CreateRentalRequest request, CancellationToken cancellationToken)
        {
            var hasGuestData = !string.IsNullOrWhiteSpace(request.GuestEmail);
            var hasRegisteredCustomer = !string.IsNullOrWhiteSpace(request.CustomerId);

            if (hasGuestData == hasRegisteredCustomer)   // both or neither
                return ApplicationResult<RentalResponse>.Failure(
                    "Provide either CustomerId or guest data (GuestEmail + GuestFullName), not both.",
                    ResultError.Validation);

            if (hasGuestData && string.IsNullOrWhiteSpace(request.GuestFullName))
                return ApplicationResult<RentalResponse>.Failure(
                    "Guest name is required for guest rentals.", ResultError.Validation);

            if (!request.VehicleId.HasValue ||
                !request.PickupLocationId.HasValue ||
                !request.DropoffLocationId.HasValue ||
                !request.ExpectedReturnTimeUtc.HasValue)
                return ApplicationResult<RentalResponse>.Failure(
                    "For a walk-in rental, VehicleId, PickupLocationId, DropoffLocationId and ExpectedReturnTimeUtc are required.",
                    ResultError.Validation);

            var now = DateTimeOffset.UtcNow;

            if (request.ExpectedReturnTimeUtc.Value <= now)
                return ApplicationResult<RentalResponse>.Failure(
                    "Expected return time must be in the future.", ResultError.Validation);

            UserEntity? customer = null;
            if (hasRegisteredCustomer)
            {
                customer = await userManager.FindByIdAsync(request.CustomerId!);
                if (customer is null || !customer.IsActive)
                    return ApplicationResult<RentalResponse>.Failure(
                        "Customer not found.", ResultError.Validation);
            }

            var vehicleResult = await vehiclesRepository.GetVehicleByIdAsync(request.VehicleId.Value, cancellationToken);
            if (!vehicleResult.IsSuccess)
                return ApplicationResult<RentalResponse>.Failure(
                    "Failed to validate vehicle.", ResultError.Internal);
            if (vehicleResult.Value is null)
                return ApplicationResult<RentalResponse>.Failure(
                    "Vehicle not found.", ResultError.Validation);
            var vehicle = vehicleResult.Value;

            var pickupResult = await locationRepository.GetByIdAsync(request.PickupLocationId.Value, cancellationToken);
            if (!pickupResult.IsSuccess)
                return ApplicationResult<RentalResponse>.Failure(
                    "Failed to validate pickup location.", ResultError.Internal);
            if (pickupResult.Value is null || !pickupResult.Value.IsActive)
                return ApplicationResult<RentalResponse>.Failure(
                    "Pickup location not found.", ResultError.Validation);

            if (request.DropoffLocationId.Value != request.PickupLocationId.Value)
            {
                var dropoffResult = await locationRepository.GetByIdAsync(request.DropoffLocationId.Value, cancellationToken);
                if (!dropoffResult.IsSuccess)
                    return ApplicationResult<RentalResponse>.Failure(
                        "Failed to validate dropoff location.", ResultError.Internal);
                if (dropoffResult.Value is null || !dropoffResult.Value.IsActive)
                    return ApplicationResult<RentalResponse>.Failure(
                        "Dropoff location not found.", ResultError.Validation);
            }

            var availabilityError = await CheckVehicleNotOutAsync(request.VehicleId.Value, cancellationToken);
            if (availabilityError is not null)
                return availabilityError;

            // The rental starts now — must not collide with upcoming reservations
            var overlapResult = await reservationsRepository.HasOverlappingReservationAsync(
                request.VehicleId.Value, now, request.ExpectedReturnTimeUtc.Value,
                excludeReservationId: null, cancellationToken);
            if (!overlapResult.IsSuccess)
                return ApplicationResult<RentalResponse>.Failure(
                    "Failed to check vehicle availability.", ResultError.Internal);
            if (overlapResult.Value)
                return ApplicationResult<RentalResponse>.Failure(
                    "Vehicle is reserved during the requested period.", ResultError.Conflict);

            var days = (int)Math.Ceiling((request.ExpectedReturnTimeUtc.Value - now).TotalDays);
            var totalCost = days * vehicle.PricePerDayInEuro;

            var rental = new RentalEntity
            {
                ReservationId = null,
                CustomerId = hasRegisteredCustomer ? request.CustomerId : null,
                GuestEmail = hasGuestData ? request.GuestEmail!.Trim() : null,
                GuestFullName = hasGuestData ? request.GuestFullName!.Trim() : null,
                GuestPhone = hasGuestData ? request.GuestPhone?.Trim() : null,
                TrackingCode = TrackingCodeGenerator.Generate(),
                VehicleId = request.VehicleId.Value,
                PickupLocationId = request.PickupLocationId.Value,
                DropoffLocationId = request.DropoffLocationId.Value,
                ActualPickupTimeUtc = now,
                ExpectedReturnTimeUtc = request.ExpectedReturnTimeUtc.Value,
                TotalCostEuro = totalCost,
                Status = RentalStatus.Active
            };

            var createdResult = await rentalsRepository.AddRentalAsync(rental, cancellationToken);
            if (!createdResult.IsSuccess || createdResult.Value is null)
                return ApplicationResult<RentalResponse>.Failure(
                    "Failed to create rental.", ResultError.Internal);

            logger.LogInformation("Walk-in rental {RentalId} created for vehicle {VehicleId} ({CustomerKind}).",
                createdResult.Value.Id, request.VehicleId.Value, hasGuestData ? "guest" : "registered");

            return await BuildResponseAsync(createdResult.Value, cancellationToken);
        }

        // --- Shared helpers ---
        private async Task<ApplicationResult<RentalResponse>?> CheckVehicleNotOutAsync(
            int vehicleId, CancellationToken cancellationToken)
        {
            var activeResult = await rentalsRepository.HasActiveRentalForVehicleAsync(vehicleId, cancellationToken);
            if (!activeResult.IsSuccess)
                return ApplicationResult<RentalResponse>.Failure(
                    "Failed to check vehicle availability.", ResultError.Internal);
            if (activeResult.Value)
                return ApplicationResult<RentalResponse>.Failure(
                    "Vehicle is currently rented out.", ResultError.Conflict);
            return null;
        }

        private async Task<ApplicationResult<RentalResponse>> BuildResponseAsync(
            RentalEntity created, CancellationToken cancellationToken)
        {
            // Re-fetch with all navigations for a complete response
            var fullResult = await rentalsRepository.GetRentalByIdAsync(created.Id, cancellationToken);
            var rental = fullResult.IsSuccess && fullResult.Value is not null ? fullResult.Value : created;

            return ApplicationResult<RentalResponse>.Success(new RentalResponse(
                rental.Id,
                rental.ReservationId,
                rental.CustomerId,
                rental.CustomerId is not null ? rental.Customer?.Email ?? string.Empty : rental.GuestEmail ?? string.Empty,
                rental.CustomerId is not null
                    ? $"{rental.Customer?.FirstName} {rental.Customer?.LastName}".Trim()
                    : rental.GuestFullName ?? string.Empty,
                rental.CustomerId is null,
                rental.TrackingCode,
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