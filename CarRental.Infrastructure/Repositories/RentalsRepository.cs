using CarRental.Domain.Entities;
using CarRental.Domain.Enums;
using CarRental.Domain.Interfaces.DbContext;
using CarRental.Domain.Interfaces.Repositories;
using CarRental.Domain.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CarRental.Infrastructure.Repositories
{
    internal class RentalsRepository(
        ILogger<RentalsRepository> logger,
        IAppDbContext context) : IRentalsRepository
    {
        public async Task<RepositoryResult<IEnumerable<RentalEntity>>> GetAllRentalsAsync(
            string? customerId,
            int? vehicleId,
            int? pickupLocationId,
            RentalStatus? status,
            DateTimeOffset? pickupFromUtc,
            DateTimeOffset? pickupToUtc,
            int? offset,
            int? limit,
            CancellationToken cancellationToken)
        {
            try
            {
                var query = context.Rentals
                    .Include(r => r.Customer)
                    .Include(r => r.Vehicle)
                        .ThenInclude(v => v.Manufacturer)
                    .Include(r => r.PickupLocation)
                    .Include(r => r.DropoffLocation)
                    .AsQueryable();

                if (customerId is not null)
                    query = query.Where(r => r.CustomerId == customerId);
                if (vehicleId.HasValue)
                    query = query.Where(r => r.VehicleId == vehicleId.Value);
                if (pickupLocationId.HasValue)
                    query = query.Where(r => r.PickupLocationId == pickupLocationId.Value);
                if (status.HasValue)
                    query = query.Where(r => r.Status == status.Value);
                if (pickupFromUtc.HasValue)
                    query = query.Where(r => r.ActualPickupTimeUtc >= pickupFromUtc.Value);
                if (pickupToUtc.HasValue)
                    query = query.Where(r => r.ActualPickupTimeUtc <= pickupToUtc.Value);

                query = query.OrderByDescending(r => r.ActualPickupTimeUtc);

                if (offset.HasValue || limit.HasValue)
                    query = query.Skip(offset ?? 0).Take(limit ?? 50);

                var rentals = await query.ToListAsync(cancellationToken);
                return RepositoryResult<IEnumerable<RentalEntity>>.Success(rentals);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error fetching rentals.");
                return RepositoryResult<IEnumerable<RentalEntity>>.Failure("Failed to fetch rentals.", ex);
            }
        }

        public async Task<RepositoryResult<RentalEntity?>> GetRentalByIdAsync(
            int rentalId, CancellationToken cancellationToken)
        {
            try
            {
                var rental = await context.Rentals
                    .Include(r => r.Customer)
                    .Include(r => r.Vehicle)
                        .ThenInclude(v => v.Manufacturer)
                    .Include(r => r.PickupLocation)
                    .Include(r => r.DropoffLocation)
                    .Include(r => r.Reservation)
                    .FirstOrDefaultAsync(r => r.Id == rentalId, cancellationToken);

                return RepositoryResult<RentalEntity?>.Success(rental);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error fetching rental {RentalId}.", rentalId);
                return RepositoryResult<RentalEntity?>.Failure("Failed to fetch rental.", ex);
            }
        }

        public async Task<RepositoryResult<RentalEntity>> AddRentalAsync(
            RentalEntity rental, CancellationToken cancellationToken)
        {
            try
            {
                context.Rentals.Add(rental);
                await context.SaveChangesAsync(cancellationToken);
                return RepositoryResult<RentalEntity>.Success(rental);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error adding rental for vehicle {VehicleId}.", rental.VehicleId);
                return RepositoryResult<RentalEntity>.Failure("Failed to add rental.", ex);
            }
        }

        public async Task<RepositoryResult<RentalEntity>> UpdateRentalAsync(
            RentalEntity rental, CancellationToken cancellationToken)
        {
            try
            {
                context.Rentals.Update(rental);
                await context.SaveChangesAsync(cancellationToken);
                return RepositoryResult<RentalEntity>.Success(rental);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating rental {RentalId}.", rental.Id);
                return RepositoryResult<RentalEntity>.Failure("Failed to update rental.", ex);
            }
        }

        public async Task<RepositoryResult<bool>> HasActiveRentalForVehicleAsync(
            int vehicleId, CancellationToken cancellationToken)
        {
            try
            {
                var exists = await context.Rentals.AnyAsync(r =>
                    r.VehicleId == vehicleId &&
                    (r.Status == RentalStatus.Active || r.Status == RentalStatus.Overdue),
                    cancellationToken);

                return RepositoryResult<bool>.Success(exists);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error checking active rental for vehicle {VehicleId}.", vehicleId);
                return RepositoryResult<bool>.Failure("Failed to check active rentals.", ex);
            }
        }

        public async Task<RepositoryResult<IEnumerable<RentalEntity>>> GetOverdueRentalsAsync(
            DateTimeOffset asOfUtc, CancellationToken cancellationToken)
        {
            try
            {
                var overdue = await context.Rentals
                    .Include(r => r.Customer)
                    .Include(r => r.Vehicle)
                    .Where(r => r.Status == RentalStatus.Active
                        && r.ActualReturnTimeUtc == null
                        && r.ExpectedReturnTimeUtc < asOfUtc)
                    .OrderBy(r => r.ExpectedReturnTimeUtc)
                    .ToListAsync(cancellationToken);

                return RepositoryResult<IEnumerable<RentalEntity>>.Success(overdue);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error fetching overdue rentals.");
                return RepositoryResult<IEnumerable<RentalEntity>>.Failure("Failed to fetch overdue rentals.", ex);
            }
        }
    }
}