using Project.Common.Options;

namespace Project.API.Extensions
{
    public static class OptionsExtension
    {
        public static IServiceCollection AddCustomOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AppSettings>(configuration.GetSection("AppSettings"));

            services.Configure<AdminAccount>(configuration.GetSection("AdminAccount"));

            return services;
        }
    }
}
