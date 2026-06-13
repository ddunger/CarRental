namespace CarRental.Domain.Interfaces.Services
{
    public interface IQRCodeService
    {
        byte[] GenerateQrCodeUri(string userName, string secretKey);

    }
}
