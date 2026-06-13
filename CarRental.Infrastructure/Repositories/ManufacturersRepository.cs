using CarRental.Domain.Entities;
using CarRental.Domain.Interfaces.DbContext;
using CarRental.Domain.Interfaces.Repositories;
using CarRental.Domain.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
namespace CarRental.Infrastructure.Repositories
{
    internal class ManufacturersRepository(ILogger<ManufacturersRepository> logger, IAppDbContext context) : IManufacturersRepository
    {

        public async Task<RepositoryResult<ManufacturerEntity?>> GetManufacturerByIdAsync(int manufacturerId, CancellationToken cancellationToken) 
        {
            try
            {
                var manufacturer = await context.Manufacturers.FirstOrDefaultAsync(m => m.Id == manufacturerId, cancellationToken);
                return RepositoryResult<ManufacturerEntity?>.Success(manufacturer);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error fetching manufacturer {manufacturerId}", manufacturerId);
                return RepositoryResult<ManufacturerEntity?>.Failure("Failed to fetch manufacturer", ex);            
            }
        }

        public async Task<RepositoryResult<ManufacturerEntity?>> AddManufacturerAsync(ManufacturerEntity manufacturer, CancellationToken cancellationToken)
        {
            try
            {
                await context.Manufacturers.AddAsync(manufacturer, cancellationToken);
                await context.SaveChangesAsync(cancellationToken);

                return RepositoryResult<ManufacturerEntity?>.Success(manufacturer);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error adding manu {manufacturer}", manufacturer.Name);
                return RepositoryResult<ManufacturerEntity?>.Failure("Failed to add manufacturer.", ex);
            }
        } 


        public async Task<RepositoryResult<ManufacturerEntity>> UpdateManufacturerAsync(
           ManufacturerEntity manufacturer, CancellationToken cancellationToken)
        {
            try
            {
                context.Manufacturers.Update(manufacturer);
                await context.SaveChangesAsync(cancellationToken);
                return RepositoryResult<ManufacturerEntity>.Success(manufacturer);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating manufacturer {SystemId}", manufacturer.Id);
                return RepositoryResult<ManufacturerEntity>.Failure("Failed to update manufacturer.", ex);
            }
        }

        public async Task<RepositoryResult<bool>> DeleteManufacturerAsync(
            int manufacturerId, CancellationToken cancellationToken)
        {
            try
            {
                var manufacturer = await context.Manufacturers
                    .FirstOrDefaultAsync(m => m.Id == manufacturerId, cancellationToken);

                if (manufacturer is null)
                    return RepositoryResult<bool>.Success(false);

                context.Manufacturers.Remove(manufacturer);
                await context.SaveChangesAsync(cancellationToken);
                return RepositoryResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting system {manufacturerId}", manufacturerId);
                return RepositoryResult<bool>.Failure("Failed to delete manufacturer.", ex);
            }
        }

        public async Task<RepositoryResult<IEnumerable<ManufacturerEntity>>> GetAllManufacturersAsync(
            int? offset, int? limit, CancellationToken cancellationToken)
        {
            try
            {
                var query = context.Manufacturers.AsQueryable();

                if (offset.HasValue && limit.HasValue)
                    query = query.Skip(offset.Value).Take(limit.Value);

                var manufacturers = await query.ToListAsync(cancellationToken);
                return RepositoryResult<IEnumerable<ManufacturerEntity>>.Success(manufacturers);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error fetching all manufacturers");
                return RepositoryResult<IEnumerable<ManufacturerEntity>>.Failure("Failed to fetch manufacturers.", ex);
            }
        }
    }
}