using CarRental.Domain.Entities;
using CarRental.Domain.Interfaces.Services;
using CarRental.Domain.Models.Mail;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;

namespace CarRental.Application.Events
{
    public record ForgottenPasswordEvent(UserEntity User, string ValidToken) : INotification;

    public class SendResetPasswordTokenHandler(ILogger<SendResetPasswordTokenHandler> logger
   , IConfiguration configuration 
   , IMailService mailService) : INotificationHandler<ForgottenPasswordEvent>
    {
        public async Task Handle(ForgottenPasswordEvent notification, CancellationToken cancellationToken)
        { 
            string resetLink = $"{configuration["ResetPassword:HostEndPoint"]}/reset-password?email={Uri.EscapeDataString(notification.User.Email!)}&token={notification.ValidToken}";


            StringBuilder emailMessage = new();
            emailMessage.AppendLine("<!DOCTYPE html>");
            emailMessage.AppendLine("<html lang=\"en\">");
            emailMessage.AppendLine("<head>");
            emailMessage.AppendLine("   <meta charset=\"UTF-8\">");
            emailMessage.AppendLine("   <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            emailMessage.AppendLine("   <title>Password reset</title>");
            emailMessage.AppendLine("   <style>");
            emailMessage.AppendLine("       body { font-family: Arial, sans-serif; background-color: #f4f4f9; padding: 20px; }");
            emailMessage.AppendLine("       .email-container { max-width: 600px; margin: 0 auto; background-color: #fff; padding: 20px; border-radius: 5px; box-shadow: 0px 0px 10px rgba(0, 0, 0, 0.1); }");
            emailMessage.AppendLine("       h1 { color: #333; }");
            emailMessage.AppendLine("       p { color: #555; }");
            emailMessage.AppendLine("       .button { background-color: #28a745; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; font-size: 16px; }");
            emailMessage.AppendLine("       .button:hover { background-color: #218838; }");
            emailMessage.AppendLine("   </style>");
            emailMessage.AppendLine("</head>");
            emailMessage.AppendLine("<body>");
            emailMessage.AppendLine("   <div class=\"email-container\">");
            emailMessage.AppendLine($"      <span style=\"text-decoration:none !important; text-decoration:none;\"><h3>Hello, {notification.User}</h3></span>");
            emailMessage.AppendLine("       <p>Since you have forgotten your password, we have received a request to reset your user account password.</p>");
            emailMessage.AppendLine("       <p>Click on the link below which will take you to the password reset website.</p>");
            emailMessage.AppendLine($"      <p><a href=\"{resetLink}\">Reset Your Password</a></p>");
            emailMessage.AppendLine($"       <p>WARNING!!! The password reset request expires {configuration["ResetPassword:TokenExpiryInMinutes"]} minutes after you receive this email.</p>");
            emailMessage.AppendLine("       <p>If you didn't request this, please ignore this email.</p>");
            emailMessage.AppendLine("       <p>Thank you, <br/>Car Rental d.o.o.</p>");
            emailMessage.AppendLine("   </div>");
            emailMessage.AppendLine("</body>");
            emailMessage.AppendLine("</html>");

            string message = emailMessage.ToString();

            MailDataModel newMailDataModel = new()
            {
                EmailToId = notification.User.Email!,
                EmailToName = notification.User.Email!,
                EmailSubject = "Password Reset Request",
                EmailBody = message
            };
            if (!await mailService.SendEmailAsync(newMailDataModel))
                logger.LogError("Error sending reset password request email with confirmation code");
        }
    }
}

