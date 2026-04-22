namespace Project.Application.Common.Interfaces.IExternalServices.ICacheServices
{
    public interface ICacheService
    {
        T Get<T>(string key);

        void Set<T>(string key, T value, TimeSpan? expiration = null);

        void Remove(string key);

        bool Exists(string key);
    }
}
