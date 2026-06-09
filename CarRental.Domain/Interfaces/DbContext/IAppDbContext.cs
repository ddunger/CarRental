using CarRental.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace CarRental.Domain.Interfaces.DbContext
{
    public interface IAppDbContext
    {
        DbSet<VehicleEntity> Vehicles { get; }

    }
}


