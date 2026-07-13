using CarRental.Domain.Entities;
using CarRental.Domain.Enums;
using CarRental.Domain.Results;

namespace CarRental.Domain.Interfaces.Repositories
{
    public interface IReservationsRepository
    {
        Task<RepositoryResult<IEnumerable<ReservationEntity>>> GetAllReservationsAsync(
            string? customerId,
            int? vehicleId,
            int? pickupLocationId,
            ReservationStatus? status,
            DateTimeOffset? pickupFromUtc,
            DateTimeOffset? pickupToUtc,
            int? offset,
            int? limit,
            CancellationToken cancellationToken);

        Task<RepositoryResult<ReservationEntity?>> GetReservationByIdAsync(
            int reservationId, CancellationToken cancellationToken);

        Task<RepositoryResult<ReservationEntity>> AddReservationAsync(
            ReservationEntity reservation, CancellationToken cancellationToken);

        Task<RepositoryResult<ReservationEntity>> UpdateReservationAsync(
            ReservationEntity reservation, CancellationToken cancellationToken);

        Task<RepositoryResult<bool>> HasOverlappingReservationAsync(
            int vehicleId,
            DateTimeOffset pickupTimeUtc,
            DateTimeOffset expectedReturnTimeUtc,
            int? excludeReservationId,
            CancellationToken cancellationToken);
    }
}