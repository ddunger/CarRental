using CarRental.Domain.Entities;
using CarRental.Domain.Interfaces.DbContext;
using CarRental.Infrastructure.DbContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarRental.Infrastructure
{
    public static class InfrastructureDI
    {
        public static IServiceCollection AddInfrastructureDI(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            services.AddDataProtection();

            var connectionString = "TODO"; //TODO

            services.AddDbContext<AppDbContext>(options => { options.UseNpgsql(connectionString); });

            services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());

            services.AddIdentity<UserEntity, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
                options.User.RequireUniqueEmail = true;
            });

            //TODO rest

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
            //using var scope = services.CreateScope();
            //var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserEntity>>();
            //var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<UserRoleEntity>>();
            //var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            //await DbSeeder.SeedRolesAsync(roleManager);
            //await DbSeeder.SeedRolePermissionsAsync(roleManager, dbContext);
            //await DbSeeder.SeedUsersAsync(userManager, dbContext, roleManager);
        }
    }
}
