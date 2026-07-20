using CarRental.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarRental.Infrastructure.DbContext.Configurations
{
    public class ReservationConfiguration : IEntityTypeConfiguration<ReservationEntity>
    {
        public void Configure(EntityTypeBuilder<ReservationEntity> builder)
        {
            builder.HasOne(r => r.Customer)
                   .WithMany()
                   .HasForeignKey(r => r.CustomerId)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired(false);

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

            builder.Property(r => r.ExpectedCostEuro)
                   .HasPrecision(10, 2);

            // Overlap/availability check: filter by vehicle + status, then time range
            builder.HasIndex(r => new { r.VehicleId, r.Status });

            // Customer-scoped listing ("my reservations")
            builder.HasIndex(r => r.CustomerId);

            // Anonymous tracking: lookup is by code, must be unique
            builder.HasIndex(r => r.TrackingCode)
                   .IsUnique();

            // Exactly one customer identity: registered XOR guest
            builder.ToTable(t => t.HasCheckConstraint(
                "CK_Reservations_CustomerOrGuest",
                "(\"CustomerId\" IS NOT NULL AND \"GuestEmail\" IS NULL) OR (\"CustomerId\" IS NULL AND \"GuestEmail\" IS NOT NULL)"));
        }
    }
}