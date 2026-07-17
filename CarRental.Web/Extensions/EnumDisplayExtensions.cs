using CarRental.Web.Dtos.Vehicles;
using CarRental.Web.Localization;

namespace CarRental.Web.Extensions
{
    public static class EnumDisplayExtensions
    {
        public static string Display(this AcrissVehicleCategory value) =>
            Resolve($"Category_{value}", value.ToString());

        public static string Display(this AcrissVehicleType value) =>
            Resolve($"Type_{value}", value.ToString());

        public static string Display(this AcrissVehicleTransmission value) =>
            Resolve($"Transmission_{value}", value.ToString());

        public static string Display(this AcrissVehicleFuel value) =>
            Resolve($"Fuel_{value}", value.ToString());

        public static string Display(this VehicleColor value) =>
            Resolve($"Color_{value}", value.ToString());

        private static string Resolve(string key, string fallback) =>
            AppStrings.ResourceManager.GetString(key) ?? fallback;

        public static string Display(this DayOfWeek value) =>
            Resolve($"Day_{value}", value.ToString());
        public static string Display(this Dtos.Reservations.ReservationStatus value) =>
            Resolve($"Status_{value}", value.ToString());
    }
}