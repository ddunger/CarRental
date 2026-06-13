using CarRental.Domain.Interfaces.Services;
using OtpNet;

namespace CarRental.Infrastructure.Services
{
    public class Totp2FAService : ITotp2FAService
    {
        public string GenerateSecretKey()
        {
            var secret = KeyGeneration.GenerateRandomKey(20);
            return Base32Encoding.ToString(secret);
        }

        public bool ValidateCode(string secretKey, string code)
        {
            var bytes = Base32Encoding.ToBytes(secretKey);
            var totp = new Totp(bytes);
            return totp.VerifyTotp(code, out _, VerificationWindow.RfcSpecifiedNetworkDelay);
        }

    }
}
