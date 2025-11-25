using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data.Contexts;

namespace Project.API.Extensions
{
    public static class DatabaseExtension
    {
        public static IServiceCollection AddCustomDb(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(option =>
            {
                option.UseSqlServer(configuration.GetConnectionString("PrimaryDbConnection"));
            });

            return services;
        }
    }
}
