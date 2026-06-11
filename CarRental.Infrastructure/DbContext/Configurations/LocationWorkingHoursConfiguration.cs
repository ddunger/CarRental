using CarRental.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarRental.Infrastructure.DbContext.Configurations
{
    public class LocationWorkingHoursConfiguration : IEntityTypeConfiguration<LocationWorkingHoursEntity>
    {
        public void Configure(EntityTypeBuilder<LocationWorkingHoursEntity> builder)
        {
            builder.HasOne(h => h.Location)
               .WithMany(l => l.WorkingHours)
               .HasForeignKey(h => h.LocationId)
               .OnDelete(DeleteBehavior.Cascade); // delete working hours if location is deleted

            // One row per day per location
            builder.HasIndex(h => new { h.LocationId, h.DayOfWeek })
                   .IsUnique();
        }
    }
}
