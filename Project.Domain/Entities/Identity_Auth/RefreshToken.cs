using Project.Domain.Entities.Base;

namespace Project.Domain.Entities.Identity_Auth
{
    public class RefreshToken : BaseEntity
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
        public RefreshToken(int userId, string token, DateTime expiresAt, string deviceInfo, string ipAddress)
        {
            UserId = userId;
            Token = token;
            ExpiresAt = expiresAt;
            DeviceInfo = deviceInfo;
            IpAddress = ipAddress;
        }
        public void Revoke() => Revoked = DateTime.UtcNow;
    }
}