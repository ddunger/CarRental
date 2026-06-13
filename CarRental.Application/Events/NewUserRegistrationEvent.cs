using CarRental.Domain.Entities;
using MediatR;

namespace CarRental.Application.Events
{
    public record class NewUserRegistrationEvent(UserEntity NewUser) : INotification;
    
}
