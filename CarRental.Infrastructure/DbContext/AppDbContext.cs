using CarRental.Domain.Entities;
using CarRental.Domain.Interfaces.DbContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace CarRental.Infrastructure.DbContext
{
    public class AppDbContext(DbContextOptions<AppDbContext> options)
                : IdentityDbContext<UserEntity, IdentityRole, string>(options), IAppDbContext
    {
        public DbSet<VehicleEntity> Vehicles { get; set; }
        public DbSet<ManufacturerEntity> Manufacturers { get; set; } 
        public DbSet<PickupLocationEntity> Locations { get; set; }
        public DbSet<ReservationEntity> Reservations { get; set; }
        public DbSet<RentalEntity> Rentals { get; set; }
        public DbSet<LocationWorkingHoursEntity> LocationWorkingHours { get; set; }
        public DbSet<LocationHolidayEntity> LocationHolidays { get; set; }
        //refactor then include:
        //public DbSet<ChangelogEntity> Changelog { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            //individual dbsets/tables are configured within the Configurations folder
        }
    }
}