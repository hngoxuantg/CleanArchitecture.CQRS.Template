namespace Project.Application.Interfaces.IServices
{
    public interface ICurrentUserService
    {
        int? UserId { get; }
        string? UserName { get; }
        bool IsAuthenticated { get; }
    }
}
