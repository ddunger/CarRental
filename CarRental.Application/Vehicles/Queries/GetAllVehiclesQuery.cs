using CarRental.Application.Vehicles.Requests;
using CarRental.Application.Vehicles.Responses;
using CarRental.Domain.Entities;
using CarRental.Domain.Enums;
using CarRental.Domain.Interfaces.Repositories;
using CarRental.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CarRental.Application.Vehicles.Queries
{
    public record GetAllVehiclesQuery(GetVehiclesRequest Request, int? Offset, int? Limit) : IRequest<ApplicationResult<IEnumerable<VehicleResponse>>>;

    public class GetAllVehiclesQueryHandler(
        ILogger<GetAllVehiclesQueryHandler> logger,
        IVehiclesRepository vehiclesRepository)
        : IRequestHandler<GetAllVehiclesQuery, ApplicationResult<IEnumerable<VehicleResponse>>>
    {
        public async Task<ApplicationResult<IEnumerable<VehicleResponse>>> Handle(
            GetAllVehiclesQuery query, CancellationToken cancellationToken)
        {
            if (query.Offset.HasValue && query.Offset < 0)
                return ApplicationResult<IEnumerable<VehicleResponse>>.Failure(
                    "Offset must be zero or greater.", ResultError.Validation);

            if (query.Limit.HasValue && query.Limit < 1)
                return ApplicationResult<IEnumerable<VehicleResponse>>.Failure(
                    "Limit must be 1 or greater.", ResultError.Validation);

            var vehiclesResult = await vehiclesRepository.GetAllVehiclesAsync(
                query.Request.ManufacturerId,
                query.Request.YearFrom,
                query.Request.YearTo,
                query.Request.PriceFrom,
                query.Request.PriceTo,
                query.Request.MaxKilometers,
                query.Request.Color,
                query.Request.Category,
                query.Request.Type,
                query.Request.Transmission,
                query.Request.Fuel,
                query.Offset,
                query.Limit,
                cancellationToken);

            if (!vehiclesResult.IsSuccess)
            {
                return ApplicationResult<IEnumerable<VehicleResponse>>.Failure(
                    "Failed to retrieve vehicles.",
                    ResultError.Internal);
            }

            var vehicles = (vehiclesResult.Value ?? Enumerable.Empty<VehicleEntity>()).ToList();
            logger.LogInformation("Fetched {Count} vehicle(s).", vehicles.Count);

            return ApplicationResult<IEnumerable<VehicleResponse>>.Success(
                vehicles.Select(v => new VehicleResponse(
                    v.Id,
                    v.ManufacturerId,
                    v.Manufacturer?.Name ?? string.Empty,
                    v.VehicleModel,
                    v.AcrissCode,
                    v.Category,
                    v.Type,
                    v.Transmission,
                    v.Fuel,
                    v.ManufacturingYear,
                    v.KilometersDriven,
                    v.EnginePowerInKw,
                    v.RegistrationPlate,
                    v.PricePerDayInEuro,
                    v.Color)));
        }
    }
}