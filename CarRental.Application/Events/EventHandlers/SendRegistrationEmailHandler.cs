using CarRental.Domain.Entities;
using CarRental.Domain.Interfaces.Services;
using CarRental.Domain.Models.Mail;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Text;

namespace CarRental.Application.Events.EventHandlers
{
    public class SendRegistrationEmailHandler(ILogger<SendRegistrationEmailHandler> logger
      , IMailService mailService
      , UserManager<UserEntity> userManager) : INotificationHandler<NewUserRegistrationEvent>
    {
        public async Task Handle(NewUserRegistrationEvent notification, CancellationToken cancellationToken)
        {
            var emailCode = await userManager.GenerateEmailConfirmationTokenAsync(notification.NewUser);

            StringBuilder emailMessage = new();
            emailMessage.AppendLine("<html>");
            emailMessage.AppendLine("<body>");
            emailMessage.AppendLine($"<p>Dear {notification.NewUser.FirstName} {notification.NewUser.LastName},</p>");
            emailMessage.AppendLine("<p>Thank you for registration with us. To verify your email address, please use the following verification code:</p>");
            emailMessage.AppendLine("<p>Please enter this code to complete your registration.</p>");
            emailMessage.AppendLine("<p>If you did not request this, please ignore this email.</p>");
            emailMessage.AppendLine($"<h2>Verification Code: {emailCode}</h2>");
            emailMessage.AppendLine("<p></p>");
            emailMessage.AppendLine("<p>Best regards</p>");
            emailMessage.AppendLine("<p><strong>Car Rental d.o.o.</strong></p>");
            emailMessage.AppendLine("</body>");
            emailMessage.AppendLine("</html>");
            string message = emailMessage.ToString();

            MailDataModel newMailDataModel = new()
            {
                EmailToId = notification.NewUser.Email!,
                EmailToName = notification.NewUser.Email!,
                EmailSubject = "Email Confirmation",
                EmailBody = message
            };
            if (!await mailService.SendEmailAsync(newMailDataModel))
                logger.LogError("Error sending confirmation email with confirmation code");
        }
    }

}
