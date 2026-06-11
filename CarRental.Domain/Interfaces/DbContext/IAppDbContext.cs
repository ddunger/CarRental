using CarRental.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace CarRental.Domain.Interfaces.DbContext
{
    public interface IAppDbContext
    {
        DbSet<VehicleEntity> Vehicles { get; }
        DbSet<ManufacturerEntity> Manufacturers { get; }
        DbSet<PickupLocation> Locations { get; }
        DbSet<ReservationEntity> Reservations { get; }
        DbSet<RentalEntity> Rentals { get; }
        
        
        // DbSet<ChangelogEntity> Changelog { get; }

    }
}


