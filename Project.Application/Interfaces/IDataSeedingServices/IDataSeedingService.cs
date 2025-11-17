namespace Project.Application.Interfaces.IDataSeedingServices
{
    public interface IDataSeedingService
    {
        Task SeedDataAsync(CancellationToken cancellationToken = default);
    }
}
