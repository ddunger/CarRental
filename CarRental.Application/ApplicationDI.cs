using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CarRental.Application
{
    public static class ApplicationDI
    {
        public static IServiceCollection AddApplicationDI(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthorization();
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ApplicationDI).Assembly));

            services.AddHttpContextAccessor();

            //services.AddScoped<IUserContext, UserContext>();

            //services.AddScoped<ITotp2faService, Totp2faService>();

            return services;
        }
    }
}
