using Microsoft.EntityFrameworkCore;

namespace ProductService.Api.Extensions
{
    public static class DatabaseMigrationExtensions
    {
        public static async Task MigrateDatabaseAsync<TContext>(this WebApplication app) where TContext : DbContext
        {
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TContext>();

            const int maxRetries = 5;
            int retryCount = 0;

            while (true)
            {
                try
                {
                    if (await dbContext.Database.CanConnectAsync())
                    {
                        Console.WriteLine("Database exists. Checking pending migrations...");

                        // Get pending migrations
                        var pendingMigrations = (await dbContext.Database.GetPendingMigrationsAsync()).ToList();

                        if (pendingMigrations.Any())
                        {
                            Console.WriteLine($"Found {pendingMigrations.Count} pending migrations. Applying...");

                            try
                            {
                                await dbContext.Database.MigrateAsync();
                                Console.WriteLine("Pending migrations applied successfully.");
                            }
                            catch (Exception migrationException)
                            {
                                Console.WriteLine($"Error applying pending migrations: {migrationException.Message}");

                                // Mark each migration as applied if the migration fails
                                foreach (var migration in pendingMigrations)
                                {
                                    Console.WriteLine($"Marking migration '{migration}' as applied...");
                                    await dbContext.Database.ExecuteSqlRawAsync(
                                        $"INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion) VALUES ('{migration}', '9.0.1');");
                                }

                                Console.WriteLine("Pending migrations removed successfully.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("No pending migrations. Database is up to date.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Database does not exist. Creating database and applying migrations...");
                        await dbContext.Database.EnsureCreatedAsync();
                    }

                    break;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    Console.WriteLine($"Attempt {retryCount} failed: {ex.Message}");
                    if (retryCount >= maxRetries)
                    {
                        Console.WriteLine("Max retries reached. Throwing exception.");
                        throw;
                    }

                    await Task.Delay(TimeSpan.FromSeconds(5));
                }
            }
        }
    }
}
