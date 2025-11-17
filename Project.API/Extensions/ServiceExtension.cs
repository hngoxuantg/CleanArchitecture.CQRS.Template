using Project.Application.Common.Interfaces.IDataSeedingServices;
using Project.Application.Common.Interfaces.IExternalServices.IStorageServices;
using Project.Application.Common.Interfaces.IExternalServices.ITokenServices;
using Project.Application.Common.Interfaces.IServices;
using Project.Domain.Interfaces.IRepositories.IBaseRepositories;
using Project.Domain.Interfaces.IRepositories.IBusinessRepositories;
using Project.Domain.Interfaces.IRepositories.IIdentity_AuthRepositories;
using Project.Domain.Interfaces.IRepositories.ISystem_LogRepositories;
using Project.Infrastructure.Data.DataSeedingServices;
using Project.Infrastructure.Data.Repositories.BaseRepositories;
using Project.Infrastructure.Data.Repositories.BusinessRepositories;
using Project.Infrastructure.Data.Repositories.Identity_AuthRepositories;
using Project.Infrastructure.Data.Repositories.System_LogRepositories;
using Project.Infrastructure.ExternalServices.StorageServices;
using Project.Infrastructure.ExternalServices.TokenServices;
using Project.Infrastructure.Services;

namespace Project.API.Extensions
{
    public static class ServiceExtension
    {
        public static IServiceCollection Register(this IServiceCollection services)
        {
            RegisterServices(services);
            RegisterRepositories(services);
            RegisterSeedData(services);
            return services;
        }
        public static IServiceCollection RegisterServices(IServiceCollection services)
        {
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            return services;
        }
        public static IServiceCollection RegisterRepositories(IServiceCollection services)
        {
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IAuditLogRepository, AuditLogRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }
        public static IServiceCollection RegisterSeedData(IServiceCollection services)
        {
            services.AddScoped<IDataSeedingService, DataSeedingService>();
            return services;
        }
        public static IServiceCollection RegisterValidator(IServiceCollection services)
        {
            return services;
        }
    }
}
