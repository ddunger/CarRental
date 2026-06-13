using CarRental.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarRental.Infrastructure.DbContext.Configurations
{
    public class VehicleConfiguration : IEntityTypeConfiguration<VehicleEntity>
    {
        public void Configure(EntityTypeBuilder<VehicleEntity> builder)
        {
            builder.HasOne(v => v.Manufacturer)
                   .WithMany()
                   .HasForeignKey(v => v.ManufacturerId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(v => v.RegistrationPlate)
                   .IsUnique();

            builder.Property(v => v.PricePerDayInEuro)
                   .HasPrecision(10, 2);

            builder.Property(v => v.AcrissCode)
                   .HasMaxLength(4)
                   .IsFixedLength();
        }
    }
}
