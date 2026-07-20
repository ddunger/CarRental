using CarRental.Domain.Constants;
using CarRental.Domain.Entities;
using CarRental.Domain.Enums;
using CarRental.Infrastructure.DbContext.Resources;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CarRental.Infrastructure.DbContext
{
    public class DbSeeder
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = [Roles.Admin, Roles.Manager, Roles.Customer];

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        public static async Task SeedUsersAsync(UserManager<UserEntity> userManager)
        {
            var users = new[]
            {
                new { Email = "admin@carrental.com",    First = "Administrator",   Last = "User",    Role = Roles.Admin    },
                new { Email = "manager@carrental.com",  First = "Manager",         Last = "User",    Role = Roles.Manager  },
                new { Email = "customer@carrental.com", First = "Customer",        Last = "User",    Role = Roles.Customer },
            };

            foreach (var u in users)
            {
                if (await userManager.FindByEmailAsync(u.Email) != null)
                    continue;

                var user = new UserEntity
                {
                    UserName = u.Email,
                    Email = u.Email,
                    FirstName = u.First,
                    LastName = u.Last,
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(user, "Password123!");
                await userManager.AddToRoleAsync(user, u.Role);
            }
        }

        public static async Task SeedManufacturersAsync(AppDbContext db)
        {
            if (await db.Manufacturers.AnyAsync())
                return;

            var manufacturers = new List<ManufacturerEntity>
            {
                new() { Name = "Toyota" },
                new() { Name = "Volkswagen" },
                new() { Name = "Ford" },
                new() { Name = "BMW" },
                new() { Name = "Mercedes-Benz" },
                new() { Name = "Audi" },
                new() { Name = "Honda" },
                new() { Name = "Hyundai" },
                new() { Name = "Kia" },
                new() { Name = "Škoda" },
                new() { Name = "Renault" },
                new() { Name = "Peugeot" },
                new() { Name = "Opel" },
                new() { Name = "Seat" },
                new() { Name = "Nissan" },
                new() { Name = "Mazda" },
            };

            await db.Manufacturers.AddRangeAsync(manufacturers);
            await db.SaveChangesAsync();
        }

        public static async Task SeedVehiclesAsync(AppDbContext db)
        {
            if (await db.Vehicles.AnyAsync())
                return;

            var manufacturers = await db.Manufacturers
                .ToDictionaryAsync(m => m.Name, m => m.Id);

            var toyotaImage = GetImage("toyotacorolla.png");
            var volkswagenImage = GetImage("volkswagengolf.png");
            var bmwImage = GetImage("bmw3series.png");
            var mercedesImage = GetImage("mercedesbenzcclass.png");
            var fordImage = GetImage("fordkuga.png");
            var hyundaiImage = GetImage("hyundaitucson.png");
            var skodaImage = GetImage("skodaoctavia.png");
            var audiImage = GetImage("audia4.png");
            var renaultImage = GetImage("renaultzoe.png");
            var kiaImage = GetImage("kiaev6.png");

            var vehicles = new List<VehicleEntity>
            {
                new() { ManufacturerId = manufacturers["Toyota"],        VehicleModel = "Corolla",  AcrissCode = "CDMR", Category = AcrissVehicleCategory.C, Type = AcrissVehicleType.D, Transmission = AcrissVehicleTransmission.M, Fuel = AcrissVehicleFuel.R, ManufacturingYear = 2022, KilometersDriven = 15000, EnginePowerInKw = 103, RegistrationPlate = "ZG-123-AA", PricePerDayInEuro = 45.00m, Color = VehicleColor.White,    ImageData= toyotaImage.ImageData, ImageContentType = toyotaImage.ImageContentType },
                new() { ManufacturerId = manufacturers["Volkswagen"],    VehicleModel = "Golf",     AcrissCode = "CDMR", Category = AcrissVehicleCategory.C, Type = AcrissVehicleType.D, Transmission = AcrissVehicleTransmission.M, Fuel = AcrissVehicleFuel.R, ManufacturingYear = 2023, KilometersDriven = 8000,  EnginePowerInKw = 110, RegistrationPlate = "ZG-456-BB", PricePerDayInEuro = 50.00m, Color = VehicleColor.Black,    ImageData= volkswagenImage.ImageData, ImageContentType = volkswagenImage.ImageContentType },
                new() { ManufacturerId = manufacturers["BMW"],           VehicleModel = "3 Series", AcrissCode = "SDMR", Category = AcrissVehicleCategory.S, Type = AcrissVehicleType.D, Transmission = AcrissVehicleTransmission.M, Fuel = AcrissVehicleFuel.R, ManufacturingYear = 2023, KilometersDriven = 12000, EnginePowerInKw = 190, RegistrationPlate = "ZG-789-CC", PricePerDayInEuro = 95.00m, Color = VehicleColor.Blue,     ImageData= bmwImage.ImageData, ImageContentType = bmwImage.ImageContentType },
                new() { ManufacturerId = manufacturers["Mercedes-Benz"], VehicleModel = "C-Class",  AcrissCode = "SDMR", Category = AcrissVehicleCategory.S, Type = AcrissVehicleType.D, Transmission = AcrissVehicleTransmission.M, Fuel = AcrissVehicleFuel.R, ManufacturingYear = 2022, KilometersDriven = 20000, EnginePowerInKw = 170, RegistrationPlate = "ZG-111-DD", PricePerDayInEuro = 90.00m, Color = VehicleColor.Silver,   ImageData= mercedesImage.ImageData, ImageContentType = mercedesImage.ImageContentType },
                new() { ManufacturerId = manufacturers["Ford"],          VehicleModel = "Kuga",     AcrissCode = "SFMR", Category = AcrissVehicleCategory.S, Type = AcrissVehicleType.F, Transmission = AcrissVehicleTransmission.M, Fuel = AcrissVehicleFuel.R, ManufacturingYear = 2021, KilometersDriven = 35000, EnginePowerInKw = 150, RegistrationPlate = "ZG-222-EE", PricePerDayInEuro = 65.00m, Color = VehicleColor.Red,      ImageData= fordImage.ImageData, ImageContentType = fordImage.ImageContentType },
                new() { ManufacturerId = manufacturers["Hyundai"],       VehicleModel = "Tucson",   AcrissCode = "SFMR", Category = AcrissVehicleCategory.S, Type = AcrissVehicleType.F, Transmission = AcrissVehicleTransmission.M, Fuel = AcrissVehicleFuel.R, ManufacturingYear = 2022, KilometersDriven = 18000, EnginePowerInKw = 132, RegistrationPlate = "ZG-333-FF", PricePerDayInEuro = 60.00m, Color = VehicleColor.White,    ImageData= hyundaiImage.ImageData, ImageContentType = hyundaiImage.ImageContentType },
                new() { ManufacturerId = manufacturers["Škoda"],         VehicleModel = "Octavia",  AcrissCode = "CDMR", Category = AcrissVehicleCategory.C, Type = AcrissVehicleType.D, Transmission = AcrissVehicleTransmission.M, Fuel = AcrissVehicleFuel.R, ManufacturingYear = 2021, KilometersDriven = 42000, EnginePowerInKw = 110, RegistrationPlate = "ZG-444-GG", PricePerDayInEuro = 48.00m, Color = VehicleColor.Gray,     ImageData= skodaImage.ImageData, ImageContentType = skodaImage.ImageContentType },
                new() { ManufacturerId = manufacturers["Audi"],          VehicleModel = "A4",       AcrissCode = "SDMR", Category = AcrissVehicleCategory.S, Type = AcrissVehicleType.D, Transmission = AcrissVehicleTransmission.M, Fuel = AcrissVehicleFuel.R, ManufacturingYear = 2023, KilometersDriven = 5000,  EnginePowerInKw = 150, RegistrationPlate = "ZG-555-HH", PricePerDayInEuro = 85.00m, Color = VehicleColor.Black,    ImageData= audiImage.ImageData, ImageContentType = audiImage.ImageContentType },
                new() { ManufacturerId = manufacturers["Renault"],       VehicleModel = "Zoe",      AcrissCode = "ECMR", Category = AcrissVehicleCategory.E, Type = AcrissVehicleType.C, Transmission = AcrissVehicleTransmission.M, Fuel = AcrissVehicleFuel.R, ManufacturingYear = 2022, KilometersDriven = 22000, EnginePowerInKw = 100, RegistrationPlate = "ZG-666-II", PricePerDayInEuro = 55.00m, Color = VehicleColor.White,    ImageData= renaultImage.ImageData, ImageContentType = renaultImage.ImageContentType },
                new() { ManufacturerId = manufacturers["Kia"],           VehicleModel = "EV6",      AcrissCode = "PWMR", Category = AcrissVehicleCategory.P, Type = AcrissVehicleType.W, Transmission = AcrissVehicleTransmission.M, Fuel = AcrissVehicleFuel.R, ManufacturingYear = 2023, KilometersDriven = 9000,  EnginePowerInKw = 228, RegistrationPlate = "ZG-777-JJ", PricePerDayInEuro = 80.00m, Color = VehicleColor.Gray,     ImageData= kiaImage.ImageData, ImageContentType = kiaImage.ImageContentType },
            };

            await db.Vehicles.AddRangeAsync(vehicles);
            await db.SaveChangesAsync();
        }


        public static async Task SeedPickupLocationsAsync(AppDbContext db)
        {
            if (await db.Locations.AnyAsync())
                return;

            var locations = new List<PickupLocationEntity>
            {
                new() { Name = "Zagreb Airport",       Address = "Ulica Rudolfa Fizira 1, 10150 Zagreb",     Latitude = 45.7429, Longitude = 16.0688 },
                new() { Name = "Zagreb City Centre",   Address = "Trg bana Josipa Jelačića 1, 10000 Zagreb", Latitude = 45.8131, Longitude = 15.9772 },
                new() { Name = "Split Airport",        Address = "21210 Kaštela, Split",                     Latitude = 43.5389, Longitude = 16.2980 },
                new() { Name = "Dubrovnik Airport",    Address = "Dr. Ante Starčevića 1, 20213 Čilipi",      Latitude = 42.5614, Longitude = 18.2682 },
            };

            await db.Locations.AddRangeAsync(locations);
            await db.SaveChangesAsync();
        }

        public static async Task SeedLocationWorkingHoursAsync(AppDbContext db)
        {
            if (await db.LocationWorkingHours.AnyAsync())
                return;

            var locations = await db.Locations.ToListAsync();

            var allHours = new List<LocationWorkingHoursEntity>();

            foreach (var location in locations)
            {
                allHours.AddRange(
                [
                    new() { LocationId = location.Id, DayOfWeek = DayOfWeek.Monday,    OpenTime = new TimeOnly(8, 0),  CloseTime = new TimeOnly(20, 0), IsClosed = false },
                    new() { LocationId = location.Id, DayOfWeek = DayOfWeek.Tuesday,   OpenTime = new TimeOnly(8, 0),  CloseTime = new TimeOnly(20, 0), IsClosed = false },
                    new() { LocationId = location.Id, DayOfWeek = DayOfWeek.Wednesday, OpenTime = new TimeOnly(8, 0),  CloseTime = new TimeOnly(20, 0), IsClosed = false },
                    new() { LocationId = location.Id, DayOfWeek = DayOfWeek.Thursday,  OpenTime = new TimeOnly(8, 0),  CloseTime = new TimeOnly(20, 0), IsClosed = false },
                    new() { LocationId = location.Id, DayOfWeek = DayOfWeek.Friday,    OpenTime = new TimeOnly(8, 0),  CloseTime = new TimeOnly(20, 0), IsClosed = false },
                    new() { LocationId = location.Id, DayOfWeek = DayOfWeek.Saturday,  OpenTime = new TimeOnly(9, 0),  CloseTime = new TimeOnly(17, 0), IsClosed = false },
                    new() { LocationId = location.Id, DayOfWeek = DayOfWeek.Sunday,    OpenTime = new TimeOnly(0, 0),  CloseTime = new TimeOnly(0, 0),  IsClosed = true  },
                ]);
            }

            await db.LocationWorkingHours.AddRangeAsync(allHours);
            await db.SaveChangesAsync();
        }



        private static (byte[]? ImageData, string? ImageContentType) GetImage(string fileName)
        {
            var data = fileName switch
            {
                "toyotacorolla.png" => Images.toyotacorolla,
                "volkswagengolf.png" => Images.volkswagengolf,
                "bmw3series.png" => Images.bmw3series,
                "mercedesbenzcclass.png" => Images.mercedesbenzcclass,
                "fordkuga.png" => Images.fordkuga,
                "hyundaitucson.png" => Images.hyundaituscon,
                "skodaoctavia.png" => Images.skodaoctavia,
                "audia4.png" => Images.audia4,
                "renaultzoe.png" => Images.renaultzoe,
                "kiaev6.png" => Images.kiaev6,
                _ => null
            };

            if (data == null)
            { 
                Debug.WriteLine($"Image not found for file name: {fileName}");
                return (null, null); 
            }
               

            return (data, "image/png");
        }
    }
}

