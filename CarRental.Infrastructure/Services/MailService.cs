using CarRental.Domain.Interfaces.Services;
using CarRental.Domain.Models.Mail;
using CarRental.Infrastructure.Configuration;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;


namespace CarRental.Infrastructure.Services
{
    public class MailService(ILogger<MailService> logger, IOptions<MailSettings> options) : IMailService
    {
        public async Task<bool> SendEmailAsync(MailDataModel mailData) 
        {
            try 
            {
                var mailSettings = options.Value;

                //TEMP
                logger.LogInformation("Attempting email. Host: {Host}, Port: {Port}, User: {User}, To: {To}",
                    mailSettings.Host, mailSettings.Port, mailSettings.UserName, mailData.EmailToId);
                //END TEMP




                MimeMessage emailMessage = new();
                MailboxAddress emailTo = new(mailData.EmailToName, mailData.EmailToId);
                emailMessage.To.Add(emailTo);
                MailboxAddress emailFrom = new(mailSettings.Name, mailSettings.EmailId);
                emailMessage.From.Add(emailFrom);
                emailMessage.Subject = mailData.EmailSubject;
                emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = mailData.EmailBody };

                using var mailClient = new SmtpClient();
                await mailClient.ConnectAsync(mailSettings.Host, mailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
                await mailClient.AuthenticateAsync(mailSettings.UserName, mailSettings.Password);
                await mailClient.SendAsync(emailMessage);
                await mailClient.DisconnectAsync(true);
                return true;
            } 
            
            catch(Exception ex)
            {
                logger.LogError("Error sending email: {error}", ex.Message);
                return false;
            }
        
        }
    }
}
