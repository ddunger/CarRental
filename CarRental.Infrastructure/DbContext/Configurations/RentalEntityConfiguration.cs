using CarRental.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarRental.Infrastructure.DbContext.Configurations
{
    public class RentalEntityConfiguration : IEntityTypeConfiguration<RentalEntity>
    {
        public void Configure(EntityTypeBuilder<RentalEntity> builder)
        {
            builder.HasOne(r => r.Customer)
                   .WithMany()
                   .HasForeignKey(r => r.CustomerId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.Vehicle)
                   .WithMany()
                   .HasForeignKey(r => r.VehicleId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.PickupLocation)
                   .WithMany()
                   .HasForeignKey(r => r.PickupLocationId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.DropoffLocation)
                   .WithMany()
                   .HasForeignKey(r => r.DropoffLocationId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Nullable FK — for rentals with no reservation, those purchased at the spot
            builder.HasOne(r => r.Reservation)
                   .WithMany()
                   .HasForeignKey(r => r.ReservationId)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired(false);

            builder.Property(r => r.TotalCostEuro)
                   .HasPrecision(10, 2);

            // Active-rental availability check: filter by vehicle + status
            builder.HasIndex(r => new { r.VehicleId, r.Status });

            // Customer-scoped listing ("my rentals")
            builder.HasIndex(r => r.CustomerId);

            // One rental per reservation (allows multiple NULLs for walk-ins)
            builder.HasIndex(r => r.ReservationId)
                   .IsUnique();

            builder.HasIndex(r => r.TrackingCode)
                .IsUnique();

            builder.ToTable(t => t.HasCheckConstraint(
                "CK_Rentals_CustomerOrGuest",
                "(\"CustomerId\" IS NOT NULL AND \"GuestEmail\" IS NULL) OR (\"CustomerId\" IS NULL AND \"GuestEmail\" IS NOT NULL)"));
        }
    }
}