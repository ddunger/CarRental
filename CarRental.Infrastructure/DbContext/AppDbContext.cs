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






        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}