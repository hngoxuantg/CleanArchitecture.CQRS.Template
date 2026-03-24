using Microsoft.EntityFrameworkCore;
using Project.Application.Common.Interfaces.IDataSeedingServices;
using Project.Infrastructure.Data.Contexts;

namespace Project.API.Extensions
{
    public static class ApplicationBuilderExtension
    {
        public static async Task<IApplicationBuilder> UseDatabaseInitialization(this IApplicationBuilder app)
        {
            await using AsyncServiceScope scope = app.ApplicationServices.CreateAsyncScope();

            ApplicationDbContext db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            IEnumerable<string> appliedMigrations = await db.Database.GetAppliedMigrationsAsync();
            bool isFirstTime = !appliedMigrations.Any();

            await db.Database.MigrateAsync();

            if (isFirstTime)
            {
                IDataSeedingService seedingService = scope.ServiceProvider.GetRequiredService<IDataSeedingService>();
                await seedingService.SeedDataAsync();
            }

            return app;
        }
    }
}
