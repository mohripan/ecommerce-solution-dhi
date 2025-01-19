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

            using (var sysConnection = new NpgsqlConnection(sysConnString))
            {
                sysConnection.Open();
                using (var createDbCmd = sysConnection.CreateCommand())
                {
                    createDbCmd.CommandText = $@"
                    SELECT 'CREATE DATABASE {targetDatabase}'
                    WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = '{targetDatabase}');
                    ";
                    createDbCmd.ExecuteNonQuery();
                }
            }

            var targetConnString = connectionString;
            using (var targetConnection = new NpgsqlConnection(targetConnString))
            {
                targetConnection.Open();

                var createMstrStatusSql = @"
                CREATE TABLE IF NOT EXISTS MstrStatus (
                    Id SERIAL PRIMARY KEY,
                    StatusName VARCHAR(100) NOT NULL,
                    CreatedOn TIMESTAMP NOT NULL DEFAULT '2025-01-01',
                    CreatedBy VARCHAR(50) NOT NULL DEFAULT 'SYS'
                );
                ";

                var createTransactionsSql = @"
                CREATE TABLE IF NOT EXISTS TransactionHistory (
                    Id SERIAL PRIMARY KEY,
                    ProductId INT NOT NULL,
                    UserId INT NOT NULL,
                    Quantity INT NOT NULL,
                    Price DOUBLE PRECISION NOT NULL,
                    TotalPrice DOUBLE PRECISION NOT NULL,
                    TransactionAt TIMESTAMP NOT NULL,
                    StatusId INT NOT NULL,
                    Remarks TEXT DEFAULT NULL,
                    CONSTRAINT FK_Transactions_MstrStatus FOREIGN KEY (StatusId) REFERENCES MstrStatus(Id) ON DELETE RESTRICT
                );
                ";

                using (var cmd = targetConnection.CreateCommand())
                {
                    cmd.CommandText = createMstrStatusSql;
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = createTransactionsSql;
                    cmd.ExecuteNonQuery();
                }

                var seedMstrStatusSql = @"
                INSERT INTO MstrStatus (Id, StatusName, CreatedOn, CreatedBy)
                VALUES
                    (1, 'Pending', '2025-01-01', 'SYS'),
                    (2, 'Completed', '2025-01-01', 'SYS'),
                    (3, 'Cancelled', '2025-01-01', 'SYS')
                ON CONFLICT (Id) DO NOTHING;
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
