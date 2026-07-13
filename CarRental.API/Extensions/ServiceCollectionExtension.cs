using CarRental.Application;
using CarRental.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Text;

namespace CarRental.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
        {
            services.AddControllers();

            services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
            });

            services.AddApplicationDI(configuration);
            services.AddInfrastructureDI(configuration, environment);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.MapInboundClaims = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudience = configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)),              
                        RoleClaimType = "role",
                        NameClaimType = JwtRegisteredClaimNames.Sub
                    };

                    //options.Events = new JwtBearerEvents //singnalR configuration
                    //{
                    //    OnMessageReceived = context =>
                    //    {
                    //        var accessToken = context.Request.Query["access_token"];
                    //        var path = context.HttpContext.Request.Path;

                    //        if (!string.IsNullOrEmpty(accessToken) &&
                    //            path.StartsWithSegments("/notifications"))
                    //        {
                    //            context.Token = accessToken;
                    //        }

                    //        return Task.CompletedTask;
                    //    }
                    //};
                });

            //services.AddSignalR();


            //basic rate limiting configuration - max 100 requests in 1 minute
            //services.AddRateLimiter(options =>
            //{
            //    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
            //        RateLimitPartition.GetFixedWindowLimiter(
            //            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            //            factory: partition => new FixedWindowRateLimiterOptions
            //            {
            //                PermitLimit = 100,
            //                Window = TimeSpan.FromMinutes(1)
            //            }));

            //    options.RejectionStatusCode = 429;
            //});




            //CORS POLICY 
            //if (environment.IsDevelopment())
            //{
            //    services.AddCors(options =>
            //    {
            //        options.AddDefaultPolicy(policy =>
            //        {
            //            policy.SetIsOriginAllowed(_ => true)
            //                  .AllowAnyHeader()
            //                  .AllowAnyMethod()
            //                  .AllowCredentials();
            //        });
            //    });
            //}
            //else
            //{
            //    services.AddCors(options =>
            //    {
            //        options.AddDefaultPolicy(policy =>
            //        {
            //            policy.WithOrigins("https://sentinemapp.blazor") //ovdje će ići url stvarne stranice, pogotovo ako ćemo Blazor isto dodati u Docker
            //                  .AllowAnyHeader()
            //                  .AllowAnyMethod()
            //                  .AllowCredentials();
            //        });
            //    });
            //}

            return services;
        }
    }
}