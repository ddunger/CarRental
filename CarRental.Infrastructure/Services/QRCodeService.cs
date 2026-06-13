using CarRental.Domain.Interfaces.Services;
using QRCoder;


namespace CarRental.Infrastructure.Services
{
    public class QRCodeService : IQRCodeService
    {
        public byte[] GenerateQrCodeUri(string userName, string secretKey)
        {
            const string ISSUER = "MarkojaAirQality";
            string otpUri = $"otpauth://totp/{Uri.EscapeDataString(ISSUER)}:{Uri.EscapeDataString(userName)}?secret={secretKey}&issuer={ISSUER}&digits=6&period=30";
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(otpUri, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);
            return qrCode.GetGraphic(20);
        }
    }
}
