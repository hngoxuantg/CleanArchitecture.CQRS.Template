using Project.Domain.Entities.Base;
using System.Security.Cryptography;

namespace Project.Domain.Entities.Identity_Auth
{
    public class RefreshToken : BaseEntity, IAggregateRoot
    {
        public int UserId { get; set; }
        public User? User { get; set; }

        public string Token { get; private set; }

        public DateTime ExpiresAt { get; private set; }

        public DateTime? Revoked { get; private set; }

        public string? DeviceInfo { get; set; }

        public string? IpAddress { get; set; }

        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

        public bool IsActive => Revoked == null && !IsExpired;

        public RefreshToken(int userId, DateTime expiresAt, string deviceInfo, string ipAddress)
        {
            UserId = userId;
            ExpiresAt = expiresAt;
            DeviceInfo = deviceInfo;
            IpAddress = ipAddress;
            Token = GenerateRefreshToken();
        }

        public void Revoke() => Revoked = DateTime.UtcNow;

        private static string GenerateRefreshToken()
        {
            byte[] randomBytes = new byte[64];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
        }
    }
}