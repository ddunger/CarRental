namespace CarRental.Web.Constants
{
    public static class ApiEndpoints
    {
        public static class Identity
        {
            public const string Register = "api/identity/register";
            public const string Confirmation = "api/identity/confirmation";
            public const string ResendConfirmation = "api/identity/resend-confirmation";
            public const string ForgotPassword = "api/identity/forgot-password";
            public const string ResetPassword = "api/identity/reset-password";
            public const string ChangePassword = "api/identity/change-password";
            public const string LoginWeb = "api/identity/login/web";
            public const string LoginMobile = "api/identity/login/mobile";
            public const string RefreshToken = "api/identity/refresh-token";
            public const string LogoutWeb = "api/identity/logout/web";
            public const string LogoutMobile = "api/identity/logout/mobile";

            public const string TwoFactorEnable = "api/identity/2fa/enable";
            public const string TwoFactorConfirm = "api/identity/2fa/confirm";
            public const string TwoFactorDisable = "api/identity/2fa/disable";
            public const string TwoFactorLogin = "api/identity/2fa/login";
            public const string TwoFactorStatus = "api/identity/2fa/status";
            public const string TwoFactorLoginRecovery = "api/identity/2fa/login/recovery";
            public const string TwoFactorDisableRecovery = "api/identity/2fa/disable/recovery";
        }

        public static class Users
        {
            public const string GetAll = "api/users";
            public const string GetById = "api/users/{0}";
            public const string Update = "api/users/{0}";
            public const string Disable2FA = "api/users/disable-2fa/{0}";
            public const string Deactivate = "api/users/deactivate/{0}";
        }

        public static class Manufacturers
        {
            public const string GetAll = "api/manufacturers";
            public const string GetById = "api/manufacturers/{0}";
            public const string Create = "api/manufacturers";
            public const string Update = "api/manufacturers/{0}";
            public const string Delete = "api/manufacturers/{0}";
        }

        public static class Vehicles
        {
            public const string Search = "api/vehicles/search";
            public const string GetById = "api/vehicles/{0}";
            public const string Create = "api/vehicles";
            public const string Update = "api/vehicles/{0}";
            public const string Delete = "api/vehicles/{0}";
        }

        public static class Locations
        {
            public const string GetAll = "api/location";
            public const string GetById = "api/location/{0}";
            public const string Create = "api/location";
            public const string Update = "api/location/{0}";
            public const string Delete = "api/location/{0}";

            public const string AddWorkingHours = "api/location/{0}/hours";
            public const string UpdateWorkingHours = "api/location/{0}/hours/{1}";
            public const string DeleteWorkingHours = "api/location/{0}/hours/{1}";

            public const string AddHoliday = "api/location/{0}/holiday";
            public const string UpdateHoliday = "api/location/{0}/holiday/{1}";
            public const string DeleteHoliday = "api/location/{0}/holiday/{1}";
        }

        public static class Reservations
        {
            public const string GetAll = "api/reservations";
            public const string GetMy = "api/reservations/my";
            public const string GetById = "api/reservations/{0}";
            public const string Create = "api/reservations";
            public const string Confirm = "api/reservations/{0}/confirm";
            public const string Cancel = "api/reservations/{0}/cancel";
            public const string NoShow = "api/reservations/{0}/no-show";
        }

        public static class Rentals
        {
            public const string GetAll = "api/rentals";
            public const string GetMy = "api/rentals/my";
            public const string GetById = "api/rentals/{0}";
            public const string Create = "api/rentals";
            public const string Return = "api/rentals/{0}/return";
        }

        public static class Tracking
        {
            public const string Get = "api/tracking/{0}";
            public const string Cancel = "api/tracking/{0}/cancel";
        }

        public const string NotificationsHub = "/notifications";
    }
}