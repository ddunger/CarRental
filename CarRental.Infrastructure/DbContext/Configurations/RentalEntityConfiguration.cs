using CarRental.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

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

        }

    }
}
