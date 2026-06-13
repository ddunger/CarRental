using CarRental.Application.Interfaces;
using CarRental.Domain.Entities;
using CarRental.Domain.Interfaces.DbContext;
using CarRental.Domain.Interfaces.Services;
using CarRental.Infrastructure.Authentication;
using CarRental.Infrastructure.Configuration;
using CarRental.Infrastructure.DbContext;
using CarRental.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CarRental.Infrastructure
{
    public static class InfrastructureDI
    {
        public static IServiceCollection AddInfrastructureDI(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            services.AddDataProtection();

            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(connectionString);

                if (environment.IsDevelopment())
                    options.EnableSensitiveDataLogging(); 
            });

            services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());

            services.AddIdentity<UserEntity, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
                options.User.RequireUniqueEmail = true;
            });

           
            services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
            services.AddTransient<IMailService, MailService>();
            services.AddScoped<ITotp2FAService, Totp2FAService>();
            services.AddScoped<IQRCodeService, QRCodeService>();
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();


            return services;
        }


        public static async Task ApplyMigrationsAsync(this IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();

            try
            {
                logger.LogInformation("Applying database migrations...");
                await db.Database.MigrateAsync();
                logger.LogInformation("Migrations applied successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Migration failed.");
                throw;
            }
        }

        public static async Task SeedDatabaseAsync(this IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserEntity>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            await DbSeeder.SeedRolesAsync(roleManager);
            await DbSeeder.SeedUsersAsync(userManager);
            await DbSeeder.SeedManufacturersAsync(db);
            await DbSeeder.SeedVehiclesAsync(db);
            await DbSeeder.SeedPickupLocationsAsync(db);
            await DbSeeder.SeedLocationWorkingHoursAsync(db);
        }
    }
}
