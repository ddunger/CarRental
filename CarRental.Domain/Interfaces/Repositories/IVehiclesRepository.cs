using CarRental.Domain.Entities;
using CarRental.Domain.Enums;
using CarRental.Domain.Results;

namespace CarRental.Domain.Interfaces.Repositories
{
    public interface IVehiclesRepository
    {
        Task<RepositoryResult<IEnumerable<VehicleEntity>>> GetAllAsync(
            int? manufacturerId,
            int? yearFrom,
            int? yearTo,
            decimal? priceFrom,
            decimal? priceTo,
            int? maxKilometers,
            VehicleColor? color,
            AcrissVehicleCategory? category,
            AcrissVehicleType? type,
            AcrissVehicleTransmission? transmission,
            AcrissVehicleFuel? fuel,
            int? offset,
            int? limit,
            CancellationToken cancellationToken);
        Task<RepositoryResult<VehicleEntity?>> GetByIdAsync(
            int vehicleId, CancellationToken cancellationToken);

        Task<RepositoryResult<VehicleEntity>> AddAsync(
            VehicleEntity vehicle, CancellationToken cancellationToken);

        Task<RepositoryResult<VehicleEntity>> UpdateAsync(
          VehicleEntity vehicle, CancellationToken cancellationToken);
        Task<RepositoryResult<bool>> DeleteAsync(
            int vehicleId, CancellationToken cancellationToken);
    }
}
