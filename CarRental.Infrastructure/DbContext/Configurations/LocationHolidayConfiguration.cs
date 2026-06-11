using CarRental.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarRental.Infrastructure.DbContext.Configurations
{
    public class LocationHolidayConfiguration : IEntityTypeConfiguration<LocationHolidayEntity>
    {
        public void Configure(EntityTypeBuilder<LocationHolidayEntity> builder)
        {
            builder.HasOne(h => h.Location)
              .WithMany(l => l.Holidays)
              .HasForeignKey(h => h.LocationId)
              .OnDelete(DeleteBehavior.Cascade);

            // One holiday entry per date per location
            builder.HasIndex(h => new { h.LocationId, h.Date })
                   .IsUnique();
        }
    }
}
