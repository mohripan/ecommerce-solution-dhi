using Npgsql;

namespace TransactionService.Api.Extensions
{
    public static class PostgresInitializer
    {
        public static void EnsureDatabaseAndTables(string connectionString)
        {
            var builder = new NpgsqlConnectionStringBuilder(connectionString);
            var targetDatabase = builder.Database;
            var sysConnString = $"Host={builder.Host};Port={builder.Port};Username={builder.Username};Password={builder.Password}";

            // Ensure the database exists
            using (var sysConnection = new NpgsqlConnection(sysConnString))
            {
                sysConnection.Open();
                using (var createDbCmd = sysConnection.CreateCommand())
                {
                    createDbCmd.CommandText = $@"
                    DO $$
                    BEGIN
                        IF NOT EXISTS (SELECT 1 FROM pg_database WHERE datname = '{targetDatabase}') THEN
                            EXECUTE 'CREATE DATABASE {targetDatabase}';
                        END IF;
                    END
                    $$;
                    ";
                    createDbCmd.ExecuteNonQuery();
                }
            }

            // Ensure tables exist and seed data
            var targetConnString = connectionString;
            using (var targetConnection = new NpgsqlConnection(targetConnString))
            {
                targetConnection.Open();

                var createMstrStatusSql = @"
                CREATE TABLE IF NOT EXISTS ""MstrStatus"" (
                    ""Id"" SERIAL PRIMARY KEY,
                    ""StatusName"" VARCHAR(100) NOT NULL,
                    ""CreatedOn"" TIMESTAMP NOT NULL DEFAULT '2025-01-01',
                    ""CreatedBy"" VARCHAR(50) NOT NULL DEFAULT 'SYS'
                );
                ";

                var createTransactionHistorySql = @"
                CREATE TABLE IF NOT EXISTS ""TransactionHistory"" (
                    ""Id"" SERIAL PRIMARY KEY,
                    ""ProductId"" INT NOT NULL,
                    ""UserId"" INT NOT NULL,
                    ""Quantity"" INT NOT NULL,
                    ""Price"" DOUBLE PRECISION NOT NULL,
                    ""TotalPrice"" DOUBLE PRECISION NOT NULL,
                    ""TransactionAt"" timestamptz NOT NULL DEFAULT NOW(),
                    ""ModifiedOn"" timestamptz NULL,
                    ""StatusId"" INT NOT NULL,
                    ""Remarks"" TEXT DEFAULT NULL,
                    CONSTRAINT FK_TransactionHistory_MstrStatus FOREIGN KEY (""StatusId"")
                        REFERENCES ""MstrStatus""(""Id"") ON DELETE RESTRICT
                );
                ";

                using (var cmd = targetConnection.CreateCommand())
                {
                    // Create MstrStatus table
                    cmd.CommandText = createMstrStatusSql;
                    cmd.ExecuteNonQuery();

                    // Create TransactionHistory table
                    cmd.CommandText = createTransactionHistorySql;
                    cmd.ExecuteNonQuery();
                }

                // Seed MstrStatus data
                var seedMstrStatusSql = @"
                        INSERT INTO ""MstrStatus"" (""Id"", ""StatusName"", ""CreatedOn"", ""CreatedBy"")
                        VALUES
                            (1, 'Pending', '2025-01-01', 'SYS'),
                            (2, 'Completed', '2025-01-01', 'SYS'),
                            (3, 'Cancelled', '2025-01-01', 'SYS')
                        ON CONFLICT (""Id"") DO NOTHING;
                    ";

                using (var cmd = targetConnection.CreateCommand())
                {
                    cmd.CommandText = seedMstrStatusSql;
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
