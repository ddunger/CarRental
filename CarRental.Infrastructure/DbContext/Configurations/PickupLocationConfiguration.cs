using CarRental.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarRental.Infrastructure.DbContext.Configurations
{
    public class PickupLocationConfiguration
     : IEntityTypeConfiguration<PickupLocationEntity>
    {
        public void Configure(EntityTypeBuilder<PickupLocationEntity> builder)
        {
            builder.Property(p => p.Address)
                   .HasMaxLength(200);

            builder.Property(p => p.Latitude)
                   .HasPrecision(9, 6);  // e.g. 45.815399

            builder.Property(p => p.Longitude)
                   .HasPrecision(9, 6);  // e.g. 15.966568
        }
    }
}
