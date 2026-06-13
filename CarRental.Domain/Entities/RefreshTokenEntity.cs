using CarRental.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarRental.Domain.Entities
{

    [Table("RefreshTokens")]
    public class RefreshTokenEntity
    {
        public Guid Id { get; set; }

        [MaxLength(100)]
        public string Token { get; set; } = null!;

        [MaxLength(50)]
        public string UserId { get; set; } = null!;
        public DateTime ExpiresAtUtc { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public ClientType ClientType { get; set; }
        public bool IsRevoked { get; set; }

        // Navigation
        public UserEntity User { get; set; } = null!;
    }
}