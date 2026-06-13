namespace CarRental.Domain.Interfaces.Services
{
    public interface ITotp2FAService
    {
        string GenerateSecretKey();
        bool ValidateCode(string secretKey, string code);
    }
}
