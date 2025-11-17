namespace Project.Application.Common.Interfaces.IServices
{
    public interface ICurrentUserService
    {
        int? UserId { get; }

        string? UserName { get; }

        string? IpAddress { get; }

        string? DeviceInfo { get; }

        bool IsAuthenticated { get; }

        IEnumerable<string> Roles { get; }
    }
}
