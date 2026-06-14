using CarRental.Domain.Entities;
using CarRental.Domain.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CarRental.Domain.Interfaces.Repositories
{
    public interface IPickupLocationRepository
    {
        Task<RepositoryResult<IEnumerable<PickupLocationEntity>>> GetAllAsync(
          int? offset, int? limit, CancellationToken cancellationToken);

        Task<RepositoryResult<PickupLocationEntity?>> GetByIdAsync(
            int locationId, CancellationToken cancellationToken);

        Task<RepositoryResult<PickupLocationEntity>> AddAsync(
            PickupLocationEntity location, CancellationToken cancellationToken);

        Task<RepositoryResult<PickupLocationEntity>> UpdateAsync(
            PickupLocationEntity location, CancellationToken cancellationToken);

        Task<RepositoryResult<bool>> DeleteAsync(
            int locationId, CancellationToken cancellationToken);

        // --- Working Hours ---
        Task<RepositoryResult<LocationWorkingHoursEntity>> AddWorkingHoursAsync(
            LocationWorkingHoursEntity workingHours, CancellationToken cancellationToken);

        Task<RepositoryResult<LocationWorkingHoursEntity?>> GetWorkingHoursByIdAsync(
            int hoursId, CancellationToken cancellationToken);

        Task<RepositoryResult<LocationWorkingHoursEntity>> UpdateWorkingHoursAsync(
            LocationWorkingHoursEntity workingHours, CancellationToken cancellationToken);

        Task<RepositoryResult<bool>> DeleteWorkingHoursAsync(
            int hoursId, CancellationToken cancellationToken);


        // --- Holidays ---
        Task<RepositoryResult<LocationHolidayEntity>> AddHolidayAsync(
            LocationHolidayEntity holiday, CancellationToken cancellationToken);

        Task<RepositoryResult<LocationHolidayEntity?>> GetHolidayByIdAsync(
            int holidayId, CancellationToken cancellationToken);

        Task<RepositoryResult<LocationHolidayEntity>> UpdateHolidayAsync(
            LocationHolidayEntity holiday, CancellationToken cancellationToken);

        Task<RepositoryResult<bool>> DeleteHolidayAsync(
            int holidayId, CancellationToken cancellationToken);        
    }
}
  