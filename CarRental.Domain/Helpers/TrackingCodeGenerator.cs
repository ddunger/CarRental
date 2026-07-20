using System.Security.Cryptography;

namespace CarRental.Domain.Helpers
{
    public static class TrackingCodeGenerator
    {
        /// <summary>
        /// Generates a cryptographically random, URL-safe tracking code.
        /// </summary>
        public static string Generate()
        {
            var bytes = RandomNumberGenerator.GetBytes(16);
            return Convert.ToBase64String(bytes)
                .Replace('+', '-')
                .Replace('/', '_')
                .TrimEnd('=');
        }
    }
}
