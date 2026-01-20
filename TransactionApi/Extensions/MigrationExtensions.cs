using Microsoft.EntityFrameworkCore;
using TransactionApi.Data;

namespace TransactionApi.Extensions
{
    public static class MigrationExtensions
    {
        public static void ApplyMigrations(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var provider = scope.ServiceProvider;

            var logger = provider.GetService<ILoggerFactory>()?.CreateLogger("DatabaseMigration");

            try
            {
                var db = provider.GetRequiredService<TransactionDbContext>();
                db.Database.Migrate();
                logger?.LogInformation("Database migrations applied successfully.");
            }
            catch (Exception ex)
            {
                logger?.LogCritical(ex, "Failed to apply database migrations. Stopping application.");
                throw;
            }
        }
    }
}
