using Project.Domain.Entities.Identity_Auth;

namespace Project.Domain.Entities.System_Log
{
    public class AuditLog
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public User? User { get; set; }
        public string? UserName { get; set; }
        public string Action { get; set; } = string.Empty;
        public string EntityType { get; set; } = string.Empty;
        public int EntityId { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;

        public string? IPAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? CorrelationId { get; set; }
        public string? Source { get; set; }
        public string? RequestPath { get; set; }
    }
}
