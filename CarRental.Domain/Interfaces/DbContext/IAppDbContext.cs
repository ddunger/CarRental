using CarRental.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace CarRental.Domain.Interfaces.DbContext
{
    public interface IAppDbContext
    {
        DbSet<VehicleEntity> Vehicles { get; }
        DbSet<ManufacturerEntity> Manufacturers { get; }
        DbSet<PickupLocationEntity> Locations { get; }
        DbSet<ReservationEntity> Reservations { get; }
        DbSet<RentalEntity> Rentals { get; }
        DbSet<LocationWorkingHoursEntity> LocationWorkingHours { get; }
        DbSet<LocationHolidayEntity> LocationHolidays { get; }
        DbSet<RefreshTokenEntity> RefreshTokens { get;}

        // DbSet<ChangelogEntity> Changelog { get; }


        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
     

    }
}


