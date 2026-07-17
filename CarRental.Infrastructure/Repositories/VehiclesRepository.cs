using CarRental.Domain.Entities;
using CarRental.Domain.Enums;
using CarRental.Domain.Interfaces.DbContext;
using CarRental.Domain.Interfaces.Repositories;
using CarRental.Domain.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CarRental.Infrastructure.Repositories
{
    internal class VehiclesRepository(
        ILogger<VehiclesRepository> logger,
        IAppDbContext context) : IVehiclesRepository
    {
        public async Task<RepositoryResult<IEnumerable<VehicleEntity>>> GetAllVehiclesAsync(
            List<int>? manufacturerIds,
            int? yearFrom,
            int? yearTo,
            decimal? priceFrom,
            decimal? priceTo,
            int? maxKilometers,
            List<VehicleColor>? colors,
            List<AcrissVehicleCategory>? categories,
            List<AcrissVehicleType>? types,
            List<AcrissVehicleTransmission>? transmissions,
            List<AcrissVehicleFuel>? fuels,
            int? offset,
            int? limit,
            CancellationToken cancellationToken)
        {
            try
            {
                var query = context.Vehicles
                    .Include(v => v.Manufacturer)
                    .AsQueryable();

                if (manufacturerIds is { Count: > 0 })
                    query = query.Where(v => manufacturerIds.Contains(v.ManufacturerId));
                if (yearFrom.HasValue)
                    query = query.Where(v => v.ManufacturingYear >= yearFrom.Value);
                if (yearTo.HasValue)
                    query = query.Where(v => v.ManufacturingYear <= yearTo.Value);
                if (priceFrom.HasValue)
                    query = query.Where(v => v.PricePerDayInEuro >= priceFrom.Value);
                if (priceTo.HasValue)
                    query = query.Where(v => v.PricePerDayInEuro <= priceTo.Value);
                if (maxKilometers.HasValue)
                    query = query.Where(v => v.KilometersDriven <= maxKilometers.Value);
                if (colors is { Count: > 0 })
                    query = query.Where(v => colors.Contains(v.Color));
                if (categories is { Count: > 0 })
                    query = query.Where(v => categories.Contains(v.Category));
                if (types is { Count: > 0 })
                    query = query.Where(v => types.Contains(v.Type));
                if (transmissions is { Count: > 0 })
                    query = query.Where(v => transmissions.Contains(v.Transmission));
                if (fuels is { Count: > 0 })
                    query = query.Where(v => fuels.Contains(v.Fuel));

                query = query.OrderBy(v => v.Id);

                if (offset.HasValue || limit.HasValue)
                    query = query.Skip(offset ?? 0).Take(limit ?? 50);

                var vehicles = await query.ToListAsync(cancellationToken);
                return RepositoryResult<IEnumerable<VehicleEntity>>.Success(vehicles);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error fetching vehicles.");
                return RepositoryResult<IEnumerable<VehicleEntity>>.Failure("Failed to fetch vehicles.", ex);
            }
        }

        public async Task<RepositoryResult<VehicleEntity?>> GetVehicleByIdAsync(
            int vehicleId, CancellationToken cancellationToken)
        {
            try
            {
                var vehicle = await context.Vehicles
                    .Include(v => v.Manufacturer)
                    .FirstOrDefaultAsync(v => v.Id == vehicleId, cancellationToken);
                return RepositoryResult<VehicleEntity?>.Success(vehicle);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error fetching vehicle {VehicleId}.", vehicleId);
                return RepositoryResult<VehicleEntity?>.Failure("Failed to fetch vehicle.", ex);
            }
        }

        public async Task<RepositoryResult<VehicleEntity>> AddVehicleAsync(
            VehicleEntity vehicle, CancellationToken cancellationToken)
        {
            try
            {
                context.Vehicles.Add(vehicle);
                await context.SaveChangesAsync(cancellationToken);
                return RepositoryResult<VehicleEntity>.Success(vehicle);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error adding vehicle {RegistrationPlate}.", vehicle.RegistrationPlate);
                return RepositoryResult<VehicleEntity>.Failure("Failed to add vehicle.", ex);
            }
        }

        public async Task<RepositoryResult<VehicleEntity>> UpdateVehicleAsync(
            VehicleEntity vehicle, CancellationToken cancellationToken)
        {
            try
            {
                context.Vehicles.Update(vehicle);
                await context.SaveChangesAsync(cancellationToken);
                return RepositoryResult<VehicleEntity>.Success(vehicle);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating vehicle {VehicleId}.", vehicle.Id);
                return RepositoryResult<VehicleEntity>.Failure("Failed to update vehicle.", ex);
            }
        }

        public async Task<RepositoryResult<bool>> DeleteVehicleAsync(
            int vehicleId, CancellationToken cancellationToken)
        {
            try
            {
                var vehicle = await context.Vehicles
                    .FirstOrDefaultAsync(v => v.Id == vehicleId, cancellationToken);
                if (vehicle is null)
                    return RepositoryResult<bool>.Success(false);
                context.Vehicles.Remove(vehicle);
                await context.SaveChangesAsync(cancellationToken);
                return RepositoryResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting vehicle {VehicleId}.", vehicleId);
                return RepositoryResult<bool>.Failure("Failed to delete vehicle.", ex);
            }
        }
    }
}

