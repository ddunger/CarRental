using CarRental.Web;
using CarRental.Web.Services.Auth;
using CarRental.Web.Services.Http;
using CarRental.Web.Services.Identity;
using CarRental.Web.Services.Locations; 
using CarRental.Web.Services.Manufacturers;
using CarRental.Web.Services.Notifications;
using CarRental.Web.Services.Reservations;
using CarRental.Web.Services.Vehicles;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddMudServices();

builder.Services.AddScoped<TokenStorageService>();
builder.Services.AddScoped<JwtAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<JwtAuthenticationStateProvider>());

builder.Services.AddTransient<AuthorizationMessageHandler>();
builder.Services.AddHttpClient("CarRentalApi", client =>
        client.BaseAddress = new Uri("https://localhost:8081/"))
    .AddHttpMessageHandler<AuthorizationMessageHandler>();

builder.Services.AddScoped<IdentityApiService>();
builder.Services.AddScoped<NotificationHubService>();
builder.Services.AddScoped<NotificationStateService>();
builder.Services.AddScoped<ManufacturerApiService>();
builder.Services.AddScoped<VehicleApiService>();
builder.Services.AddScoped<LocationApiService>();
builder.Services.AddScoped<ReservationApiService>();


builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("CarRentalApi"));

builder.Services.AddLocalization();


var host = builder.Build();

var js = host.Services.GetRequiredService<IJSRuntime>();
var storedCulture = await js.InvokeAsync<string?>("localStorage.getItem", "carrental_culture");
var culture = new System.Globalization.CultureInfo(storedCulture ?? "en");
System.Globalization.CultureInfo.DefaultThreadCurrentCulture = culture;
System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = culture;

await host.RunAsync();
