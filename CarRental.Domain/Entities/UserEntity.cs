using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CarRental.Domain.Entities
{
    public class UserEntity : IdentityUser
    {
        [StringLength(50)]
        public string? FirstName { get; set; }

        [StringLength(50)]
        public string? LastName { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = null;
        [StringLength(50)]
        public string? TwoFactorSecretKey { get; set; } = string.Empty; 
    }
}
