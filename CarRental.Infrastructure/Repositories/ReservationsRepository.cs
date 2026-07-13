using CarRental.Domain.Entities;
using CarRental.Domain.Enums;
using CarRental.Domain.Interfaces.DbContext;
using CarRental.Domain.Interfaces.Repositories;
using CarRental.Domain.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CarRental.Infrastructure.Repositories
{
    internal class ReservationsRepository(
        ILogger<ReservationsRepository> logger,
        IAppDbContext context) : IReservationsRepository
    {
        public async Task<RepositoryResult<IEnumerable<ReservationEntity>>> GetAllReservationsAsync(
            string? customerId,
            int? vehicleId,
            int? pickupLocationId,
            ReservationStatus? status,
            DateTimeOffset? pickupFromUtc,
            DateTimeOffset? pickupToUtc,
            int? offset,
            int? limit,
            CancellationToken cancellationToken)
        {
            try
            {
                var query = context.Reservations
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
                    query = query.Where(r => r.PickupTimeUtc >= pickupFromUtc.Value);
                if (pickupToUtc.HasValue)
                    query = query.Where(r => r.PickupTimeUtc <= pickupToUtc.Value);

                query = query.OrderByDescending(r => r.PickupTimeUtc);

                if (offset.HasValue || limit.HasValue)
                    query = query.Skip(offset ?? 0).Take(limit ?? 50);

                var reservations = await query.ToListAsync(cancellationToken);
                return RepositoryResult<IEnumerable<ReservationEntity>>.Success(reservations);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error fetching reservations.");
                return RepositoryResult<IEnumerable<ReservationEntity>>.Failure("Failed to fetch reservations.", ex);
            }
        }

        public async Task<RepositoryResult<ReservationEntity?>> GetReservationByIdAsync(
            int reservationId, CancellationToken cancellationToken)
        {
            try
            {
                var reservation = await context.Reservations
                    .Include(r => r.Customer)
                    .Include(r => r.Vehicle)
                        .ThenInclude(v => v.Manufacturer)
                    .Include(r => r.PickupLocation)
                    .Include(r => r.DropoffLocation)
                    .FirstOrDefaultAsync(r => r.Id == reservationId, cancellationToken);

                return RepositoryResult<ReservationEntity?>.Success(reservation);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error fetching reservation {ReservationId}.", reservationId);
                return RepositoryResult<ReservationEntity?>.Failure("Failed to fetch reservation.", ex);
            }
        }

        public async Task<RepositoryResult<ReservationEntity>> AddReservationAsync(
            ReservationEntity reservation, CancellationToken cancellationToken)
        {
            try
            {
                context.Reservations.Add(reservation);
                await context.SaveChangesAsync(cancellationToken);
                return RepositoryResult<ReservationEntity>.Success(reservation);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error adding reservation for vehicle {VehicleId}.", reservation.VehicleId);
                return RepositoryResult<ReservationEntity>.Failure("Failed to add reservation.", ex);
            }
        }

        public async Task<RepositoryResult<ReservationEntity>> UpdateReservationAsync(
            ReservationEntity reservation, CancellationToken cancellationToken)
        {
            try
            {
                context.Reservations.Update(reservation);
                await context.SaveChangesAsync(cancellationToken);
                return RepositoryResult<ReservationEntity>.Success(reservation);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating reservation {ReservationId}.", reservation.Id);
                return RepositoryResult<ReservationEntity>.Failure("Failed to update reservation.", ex);
            }
        }

        public async Task<RepositoryResult<bool>> HasOverlappingReservationAsync(
            int vehicleId,
            DateTimeOffset pickupTimeUtc,
            DateTimeOffset expectedReturnTimeUtc,
            int? excludeReservationId,
            CancellationToken cancellationToken)
        {
            try
            {
                var query = context.Reservations.Where(r =>
                    r.VehicleId == vehicleId &&
                    (r.Status == ReservationStatus.Pending || r.Status == ReservationStatus.Confirmed) &&
                    r.PickupTimeUtc < expectedReturnTimeUtc &&
                    pickupTimeUtc < r.ExpectedReturnTimeUtc);

                if (excludeReservationId.HasValue)
                    query = query.Where(r => r.Id != excludeReservationId.Value);

                var exists = await query.AnyAsync(cancellationToken);
                return RepositoryResult<bool>.Success(exists);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error checking reservation overlap for vehicle {VehicleId}.", vehicleId);
                return RepositoryResult<bool>.Failure("Failed to check availability.", ex);
            }
        }
    }
}