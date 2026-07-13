using CarRental.Infrastructure;
using Scalar.AspNetCore;

namespace CarRental.API.Extensions
{
    public static class WebApplicationExtensions
    {
        public static async Task <WebApplication> ConfigurePipelineAsync(this WebApplication app)
        {
            await app.Services.ApplyMigrationsAsync();
            await app.Services.SeedDatabaseAsync();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference(options =>
                {
                    options.Title = "CarRental API";
                    options.Authentication = new ScalarAuthenticationOptions
                    {
                        PreferredSecuritySchemes = new[] { "OAuth2Password" }
                    };
                });
            }

            app.UseHttpsRedirection();
            //app.UseRateLimiter();
            app.UseCors();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            return app;
        }
    }
}
