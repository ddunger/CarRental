using CarRental.Domain.Entities;
using CarRental.Domain.Enums;
using CarRental.Domain.Results;

namespace CarRental.Domain.Interfaces.Repositories
{
    public interface IRentalsRepository
    {
        Task<RepositoryResult<IEnumerable<RentalEntity>>> GetAllRentalsAsync(
            string? customerId,
            int? vehicleId,
            int? pickupLocationId,
            RentalStatus? status,
            DateTimeOffset? pickupFromUtc,
            DateTimeOffset? pickupToUtc,
            int? offset,
            int? limit,
            CancellationToken cancellationToken);

        Task<RepositoryResult<RentalEntity?>> GetRentalByIdAsync(
            int rentalId, CancellationToken cancellationToken);

        Task<RepositoryResult<RentalEntity>> AddRentalAsync(
            RentalEntity rental, CancellationToken cancellationToken);

        Task<RepositoryResult<RentalEntity>> UpdateRentalAsync(
            RentalEntity rental, CancellationToken cancellationToken);

        Task<RepositoryResult<bool>> HasActiveRentalForVehicleAsync(
            int vehicleId, CancellationToken cancellationToken);

        Task<RepositoryResult<IEnumerable<RentalEntity>>> GetOverdueRentalsAsync(
            DateTimeOffset asOfUtc, CancellationToken cancellationToken);
        Task<RepositoryResult<RentalEntity?>> GetRentalByTrackingCodeAsync(
            string trackingCode, CancellationToken cancellationToken);

        Task<RepositoryResult<RentalEntity?>> GetRentalByReservationIdAsync(
            int reservationId, CancellationToken cancellationToken);
    }
}