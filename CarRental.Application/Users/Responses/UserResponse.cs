namespace CarRental.Application.Users.Responses
{
    public record UserResponse(
        string Id, 
        string Email, 
        string? Role, 
        string? FirstName, 
        string? LastName, 
        bool IsActive, 
        bool Has2faEnabled, 
        DateTimeOffset CreatedAt,
        DateTimeOffset UpdatedAt);   
    
}
