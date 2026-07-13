using CarRental.Application.Reservations.Requests;
using CarRental.Application.Reservations.Responses;
using CarRental.Domain.Constants;
using CarRental.Domain.Entities;
using CarRental.Domain.Enums;
using CarRental.Domain.Interfaces.Application;
using CarRental.Domain.Interfaces.Repositories;
using CarRental.Domain.Results;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CarRental.Application.Reservations.Commands
{
    public record CreateReservationCommand(CreateReservationRequest Request)
        : IRequest<ApplicationResult<ReservationResponse>>;

    public class CreateReservationCommandHandler(
        ILogger<CreateReservationCommandHandler> logger,
        IUserContext userContext,
        IReservationsRepository reservationsRepository,
        IRentalsRepository rentalsRepository,
        IVehiclesRepository vehiclesRepository,
        IPickupLocationRepository locationRepository,
        UserManager<UserEntity> userManager)
        : IRequestHandler<CreateReservationCommand, ApplicationResult<ReservationResponse>>
    {
        public async Task<ApplicationResult<ReservationResponse>> Handle(
            CreateReservationCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request;

            var currentUser = userContext.GetCurrentUser();
            if (currentUser is null)
                return ApplicationResult<ReservationResponse>.Failure("Unauthorized", ResultError.Unauthorized);

            // --- Resolve the customer ---
            var isStaff = currentUser.IsInRole(Roles.Admin) || currentUser.IsInRole(Roles.Manager);
            var customerId = isStaff && !string.IsNullOrWhiteSpace(request.CustomerId)
                ? request.CustomerId
                : currentUser.Id;

            var customer = await userManager.FindByIdAsync(customerId);
            if (customer is null || !customer.IsActive)
                return ApplicationResult<ReservationResponse>.Failure(
                    "Customer not found.", ResultError.Validation);

            // --- Validate times ---
            if (request.PickupTimeUtc >= request.ExpectedReturnTimeUtc)
                return ApplicationResult<ReservationResponse>.Failure(
                    "Pickup time must be before return time.", ResultError.Validation);

            if (request.PickupTimeUtc <= DateTimeOffset.UtcNow)
                return ApplicationResult<ReservationResponse>.Failure(
                    "Pickup time must be in the future.", ResultError.Validation);

            // --- Validate vehicle ---
            var vehicleResult = await vehiclesRepository.GetVehicleByIdAsync(request.VehicleId, cancellationToken);
            if (!vehicleResult.IsSuccess)
                return ApplicationResult<ReservationResponse>.Failure(
                    "Failed to validate vehicle.", ResultError.Internal);
            if (vehicleResult.Value is null)
                return ApplicationResult<ReservationResponse>.Failure(
                    "Vehicle not found.", ResultError.Validation);
            var vehicle = vehicleResult.Value;

            // --- Validate locations ---
            var pickupLocationResult = await locationRepository.GetByIdAsync(request.PickupLocationId, cancellationToken);
            if (!pickupLocationResult.IsSuccess)
                return ApplicationResult<ReservationResponse>.Failure(
                    "Failed to validate pickup location.", ResultError.Internal);
            if (pickupLocationResult.Value is null || !pickupLocationResult.Value.IsActive)
                return ApplicationResult<ReservationResponse>.Failure(
                    "Pickup location not found.", ResultError.Validation);
            var pickupLocation = pickupLocationResult.Value;

            PickupLocationEntity dropoffLocation;
            if (request.DropoffLocationId == request.PickupLocationId)
            {
                dropoffLocation = pickupLocation;
            }
            else
            {
                var dropoffLocationResult = await locationRepository.GetByIdAsync(request.DropoffLocationId, cancellationToken);
                if (!dropoffLocationResult.IsSuccess)
                    return ApplicationResult<ReservationResponse>.Failure(
                        "Failed to validate dropoff location.", ResultError.Internal);
                if (dropoffLocationResult.Value is null || !dropoffLocationResult.Value.IsActive)
                    return ApplicationResult<ReservationResponse>.Failure(
                        "Dropoff location not found.", ResultError.Validation);
                dropoffLocation = dropoffLocationResult.Value;
            }

            // --- Validate opening hours ---
            var pickupHoursError = ValidateWithinOpeningHours(pickupLocation, request.PickupTimeUtc, "Pickup");
            if (pickupHoursError is not null)
                return ApplicationResult<ReservationResponse>.Failure(pickupHoursError, ResultError.Validation);

            var returnHoursError = ValidateWithinOpeningHours(dropoffLocation, request.ExpectedReturnTimeUtc, "Return");
            if (returnHoursError is not null)
                return ApplicationResult<ReservationResponse>.Failure(returnHoursError, ResultError.Validation);

            // --- Availability ---
            var overlapResult = await reservationsRepository.HasOverlappingReservationAsync(
                request.VehicleId, request.PickupTimeUtc, request.ExpectedReturnTimeUtc,
                excludeReservationId: null, cancellationToken);
            if (!overlapResult.IsSuccess)
                return ApplicationResult<ReservationResponse>.Failure(
                    "Failed to check vehicle availability.", ResultError.Internal);
            if (overlapResult.Value)
                return ApplicationResult<ReservationResponse>.Failure(
                    "Vehicle is already reserved for the requested period.", ResultError.Conflict);

            var activeRentalResult = await rentalsRepository.HasActiveRentalForVehicleAsync(
                request.VehicleId, cancellationToken);
            if (!activeRentalResult.IsSuccess)
                return ApplicationResult<ReservationResponse>.Failure(
                    "Failed to check vehicle availability.", ResultError.Internal);
            if (activeRentalResult.Value && request.PickupTimeUtc <= DateTimeOffset.UtcNow.AddDays(1))
                return ApplicationResult<ReservationResponse>.Failure(
                    "Vehicle is currently rented out.", ResultError.Conflict);

            // --- Cost ---
            var days = (int)Math.Ceiling((request.ExpectedReturnTimeUtc - request.PickupTimeUtc).TotalDays);
            var expectedCost = days * vehicle.PricePerDayInEuro;

            // --- Create ---
            var reservation = new ReservationEntity
            {
                CustomerId = customerId,
                VehicleId = request.VehicleId,
                PickupLocationId = request.PickupLocationId,
                DropoffLocationId = request.DropoffLocationId,
                PickupTimeUtc = request.PickupTimeUtc,
                ExpectedReturnTimeUtc = request.ExpectedReturnTimeUtc,
                ExpectedCostEuro = expectedCost,
                Status = ReservationStatus.Pending,
                CreatedAtUtc = DateTimeOffset.UtcNow
            };

            var createdResult = await reservationsRepository.AddReservationAsync(reservation, cancellationToken);
            if (!createdResult.IsSuccess || createdResult.Value is null)
                return ApplicationResult<ReservationResponse>.Failure(
                    "Failed to create reservation.", ResultError.Internal);

            var created = createdResult.Value;
            logger.LogInformation("Reservation {ReservationId} created for vehicle {VehicleId} by user {UserId}.",
                created.Id, created.VehicleId, currentUser.Id);

            return ApplicationResult<ReservationResponse>.Success(new ReservationResponse(
                created.Id,
                created.CustomerId,
                customer.Email ?? string.Empty,
                $"{customer.FirstName} {customer.LastName}".Trim(),
                created.VehicleId,
                $"{vehicle.Manufacturer?.Name} {vehicle.VehicleModel}".Trim(),
                vehicle.RegistrationPlate,
                created.PickupLocationId,
                pickupLocation.Name,
                created.DropoffLocationId,
                dropoffLocation.Name,
                created.PickupTimeUtc,
                created.ExpectedReturnTimeUtc,
                created.ExpectedCostEuro,
                created.Status,
                created.CreatedAtUtc,
                created.CancelledAtUtc));
        }

        private static string? ValidateWithinOpeningHours(
            PickupLocationEntity location, DateTimeOffset timeUtc, string label)
        {
            var date = DateOnly.FromDateTime(timeUtc.UtcDateTime);
            var time = TimeOnly.FromDateTime(timeUtc.UtcDateTime);

            // Holiday entry for the exact date overrides regular working hours
            var holiday = location.Holidays.FirstOrDefault(h => h.Date == date);
            if (holiday is not null)
            {
                if (holiday.IsClosed || holiday.OpenTime is null || holiday.CloseTime is null)
                    return $"{label} location is closed on {date:yyyy-MM-dd} ({holiday.HolidayName ?? "holiday"}).";

                if (time < holiday.OpenTime || time > holiday.CloseTime)
                    return $"{label} time is outside holiday opening hours ({holiday.OpenTime}–{holiday.CloseTime}).";

                return null;
            }

            var hours = location.WorkingHours.FirstOrDefault(w => w.DayOfWeek == timeUtc.UtcDateTime.DayOfWeek);
            if (hours is null || hours.IsClosed)
                return $"{label} location is closed on {timeUtc.UtcDateTime.DayOfWeek}s.";

            if (time < hours.OpenTime || time > hours.CloseTime)
                return $"{label} time is outside opening hours ({hours.OpenTime}–{hours.CloseTime}).";

            return null;
        }
    }
}