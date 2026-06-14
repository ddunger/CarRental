using CarRental.Domain.Entities;
using CarRental.Domain.Interfaces.DbContext;
using CarRental.Domain.Interfaces.Repositories;
using CarRental.Domain.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CarRental.Infrastructure.Repositories
{
    internal class PickupLocationRepository(
        ILogger<PickupLocationRepository> logger,
        IAppDbContext context) : IPickupLocationRepository
    {
        // --- Location ---

        public async Task<RepositoryResult<IEnumerable<PickupLocationEntity>>> GetAllAsync(
            int? offset, int? limit, CancellationToken cancellationToken)
        {
            try
            {
                var query = context.Locations.AsQueryable();
                if (offset.HasValue && limit.HasValue)
                    query = query.Skip(offset.Value).Take(limit.Value);
                var locations = await query
                    .Include(l => l.WorkingHours)
                    .Include(l => l.Holidays)
                    .ToListAsync(cancellationToken);
                return RepositoryResult<IEnumerable<PickupLocationEntity>>.Success(locations);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error fetching all locations.");
                return RepositoryResult<IEnumerable<PickupLocationEntity>>.Failure("Failed to fetch locations.", ex);
            }
        }

        public async Task<RepositoryResult<PickupLocationEntity?>> GetByIdAsync(
            int locationId, CancellationToken cancellationToken)
        {
            try
            {
                var location = await context.Locations
                    .Include(l => l.WorkingHours)
                    .Include(l => l.Holidays)
                    .FirstOrDefaultAsync(l => l.Id == locationId, cancellationToken);
                return RepositoryResult<PickupLocationEntity?>.Success(location);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error fetching location {LocationId}.", locationId);
                return RepositoryResult<PickupLocationEntity?>.Failure("Failed to fetch location.", ex);
            }
        }

        public async Task<RepositoryResult<PickupLocationEntity>> AddAsync(
            PickupLocationEntity location, CancellationToken cancellationToken)
        {
            try
            {
                context.Locations.Add(location);
                await context.SaveChangesAsync(cancellationToken);
                return RepositoryResult<PickupLocationEntity>.Success(location);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error adding location {Name}.", location.Name);
                return RepositoryResult<PickupLocationEntity>.Failure("Failed to add location.", ex);
            }
        }

        public async Task<RepositoryResult<PickupLocationEntity>> UpdateAsync(
            PickupLocationEntity location, CancellationToken cancellationToken)
        {
            try
            {
                context.Locations.Update(location);
                await context.SaveChangesAsync(cancellationToken);
                return RepositoryResult<PickupLocationEntity>.Success(location);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating location {LocationId}.", location.Id);
                return RepositoryResult<PickupLocationEntity>.Failure("Failed to update location.", ex);
            }
        }

        public async Task<RepositoryResult<bool>> DeleteAsync(
            int locationId, CancellationToken cancellationToken)
        {
            try
            {
                var location = await context.Locations
                    .FirstOrDefaultAsync(l => l.Id == locationId, cancellationToken);
                if (location is null)
                    return RepositoryResult<bool>.Success(false);
                context.Locations.Remove(location);
                await context.SaveChangesAsync(cancellationToken);
                return RepositoryResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting location {LocationId}.", locationId);
                return RepositoryResult<bool>.Failure("Failed to delete location.", ex);
            }
        }

        // --- Working Hours ---

        public async Task<RepositoryResult<LocationWorkingHoursEntity>> AddWorkingHoursAsync(
            LocationWorkingHoursEntity workingHours, CancellationToken cancellationToken)
        {
            try
            {
                context.LocationWorkingHours.Add(workingHours);
                await context.SaveChangesAsync(cancellationToken);
                return RepositoryResult<LocationWorkingHoursEntity>.Success(workingHours);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error adding working hours for location {LocationId}.", workingHours.LocationId);
                return RepositoryResult<LocationWorkingHoursEntity>.Failure("Failed to add working hours.", ex);
            }
        }

        public async Task<RepositoryResult<LocationWorkingHoursEntity?>> GetWorkingHoursByIdAsync(
            int hoursId, CancellationToken cancellationToken)
        {
            try
            {
                var hours = await context.LocationWorkingHours
                    .FirstOrDefaultAsync(h => h.Id == hoursId, cancellationToken);
                return RepositoryResult<LocationWorkingHoursEntity?>.Success(hours);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error fetching working hours {HoursId}.", hoursId);
                return RepositoryResult<LocationWorkingHoursEntity?>.Failure("Failed to fetch working hours.", ex);
            }
        }

        public async Task<RepositoryResult<LocationWorkingHoursEntity>> UpdateWorkingHoursAsync(
            LocationWorkingHoursEntity workingHours, CancellationToken cancellationToken)
        {
            try
            {
                context.LocationWorkingHours.Update(workingHours);
                await context.SaveChangesAsync(cancellationToken);
                return RepositoryResult<LocationWorkingHoursEntity>.Success(workingHours);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating working hours {HoursId}.", workingHours.Id);
                return RepositoryResult<LocationWorkingHoursEntity>.Failure("Failed to update working hours.", ex);
            }
        }

        public async Task<RepositoryResult<bool>> DeleteWorkingHoursAsync(
            int hoursId, CancellationToken cancellationToken)
        {
            try
            {
                var hours = await context.LocationWorkingHours
                    .FirstOrDefaultAsync(h => h.Id == hoursId, cancellationToken);
                if (hours is null)
                    return RepositoryResult<bool>.Success(false);
                context.LocationWorkingHours.Remove(hours);
                await context.SaveChangesAsync(cancellationToken);
                return RepositoryResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting working hours {HoursId}.", hoursId);
                return RepositoryResult<bool>.Failure("Failed to delete working hours.", ex);
            }
        }

        // --- Holidays ---

        public async Task<RepositoryResult<LocationHolidayEntity>> AddHolidayAsync(
            LocationHolidayEntity holiday, CancellationToken cancellationToken)
        {
            try
            {
                context.LocationHolidays.Add(holiday);
                await context.SaveChangesAsync(cancellationToken);
                return RepositoryResult<LocationHolidayEntity>.Success(holiday);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error adding holiday for location {LocationId}.", holiday.LocationId);
                return RepositoryResult<LocationHolidayEntity>.Failure("Failed to add holiday.", ex);
            }
        }

        public async Task<RepositoryResult<LocationHolidayEntity?>> GetHolidayByIdAsync(
            int holidayId, CancellationToken cancellationToken)
        {
            try
            {
                var holiday = await context.LocationHolidays
                    .FirstOrDefaultAsync(h => h.Id == holidayId, cancellationToken);
                return RepositoryResult<LocationHolidayEntity?>.Success(holiday);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error fetching holiday {HolidayId}.", holidayId);
                return RepositoryResult<LocationHolidayEntity?>.Failure("Failed to fetch holiday.", ex);
            }
        }

        public async Task<RepositoryResult<LocationHolidayEntity>> UpdateHolidayAsync(
            LocationHolidayEntity holiday, CancellationToken cancellationToken)
        {
            try
            {
                context.LocationHolidays.Update(holiday);
                await context.SaveChangesAsync(cancellationToken);
                return RepositoryResult<LocationHolidayEntity>.Success(holiday);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating holiday {HolidayId}.", holiday.Id);
                return RepositoryResult<LocationHolidayEntity>.Failure("Failed to update holiday.", ex);
            }
        }

        public async Task<RepositoryResult<bool>> DeleteHolidayAsync(
            int holidayId, CancellationToken cancellationToken)
        {
            try
            {
                var holiday = await context.LocationHolidays
                    .FirstOrDefaultAsync(h => h.Id == holidayId, cancellationToken);
                if (holiday is null)
                    return RepositoryResult<bool>.Success(false);
                context.LocationHolidays.Remove(holiday);
                await context.SaveChangesAsync(cancellationToken);
                return RepositoryResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting holiday {HolidayId}.", holidayId);
                return RepositoryResult<bool>.Failure("Failed to delete holiday.", ex);
            }
        }
    }
}