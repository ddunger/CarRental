using CarRental.Domain.Entities;
using CarRental.Domain.Results;

namespace CarRental.Domain.Interfaces.Repositories
{
    public interface IManufacturersRepository
    {
        Task<RepositoryResult<ManufacturerEntity?>> GetManufacturerByIdAsync
            (int manufacturerId, CancellationToken cancellationToken);

        Task<RepositoryResult<ManufacturerEntity?>> AddManufacturerAsync
            (ManufacturerEntity manufacturer, CancellationToken cancellationToken);

        Task<RepositoryResult<ManufacturerEntity>> UpdateManufacturerAsync(
           ManufacturerEntity manufacturer, CancellationToken cancellationToken);

        Task<RepositoryResult<bool>> DeleteManufacturerAsync(
            int manufacturerId, CancellationToken cancellationToken);

        Task<RepositoryResult<IEnumerable<ManufacturerEntity>>> GetAllManufacturersAsync(
            int? offset, int? limit, CancellationToken cancellationToken);     
    }
} 