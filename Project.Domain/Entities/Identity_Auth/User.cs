using Microsoft.AspNetCore.Identity;
using Project.Domain.Entities.System_Log;

namespace Project.Domain.Entities.Identity_Auth
{
    public class User : IdentityUser<int>
    {
        public string FullName { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.Now;
        public int? CreatedBy { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? UpdatedBy { get; set; }
        public byte[]? RowVersion { get; set; }


        private readonly List<RefreshToken> _refreshTokens = new List<RefreshToken>();
        public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();


        private readonly List<AuditLog> _auditLogs = new List<AuditLog>();
        public IReadOnlyCollection<AuditLog> AuditLogs => _auditLogs.AsReadOnly();
    }
}
