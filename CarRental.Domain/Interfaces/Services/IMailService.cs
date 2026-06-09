using CarRental.Domain.Models.Mail;

namespace CarRental.Domain.Interfaces.Services
{
    public interface IMailService
    {
        Task<bool> SendEmailAsync(MailDataModel mailData);
    }
}
