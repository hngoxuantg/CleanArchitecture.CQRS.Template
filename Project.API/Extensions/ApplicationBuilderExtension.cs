using Microsoft.EntityFrameworkCore;
using Project.Application.Common.Interfaces.IDataSeedingServices;
using Project.Infrastructure.Data.Contexts;

namespace Project.API.Extensions
{
    public static class ApplicationBuilderExtension
    {
        public static async Task<IApplicationBuilder> UseDatabaseInitialization(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();

            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await db.Database.MigrateAsync();

            var seedingService = scope.ServiceProvider.GetRequiredService<IDataSeedingService>();
            await seedingService.SeedDataAsync();

            return app;
        }
    }
}
