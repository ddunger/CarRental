using CarRental.Application.Common.User;

namespace CarRental.Domain.Interfaces.Application
{
    public interface IUserContext
    {
        CurrentUser? GetCurrentUser();
    }
}
