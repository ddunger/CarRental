using CarRental.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarRental.Infrastructure.DbContext.Configurations
{
    public class ManufacturerConfiguration
     : IEntityTypeConfiguration<ManufacturerEntity>
    {
        public void Configure(EntityTypeBuilder<ManufacturerEntity> builder)
        {
            builder.HasIndex(m => m.Name)
                   .IsUnique();

            builder.Property(m => m.Name)
                   .HasMaxLength(100);
        }

    }
}
