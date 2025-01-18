using Microsoft.EntityFrameworkCore;

namespace UserService.Api.Extensions
{
    public static class DatabaseMigrationExtensions
    {
        public static async Task MigrateDatabaseAsync<TContext>(this WebApplication app) where TContext : DbContext
        {
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TContext>();

            try
            {
                if (await dbContext.Database.CanConnectAsync())
                {
                    Console.WriteLine("Database exists. Applying migrations...");
                    await dbContext.Database.MigrateAsync();
                }
                else
                {
                    Console.WriteLine("Database does not exist. Creating database and applying migrations...");
                    await dbContext.Database.EnsureCreatedAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while migrating the database: {ex.Message}");
                throw;
            }
        }
    }
}
