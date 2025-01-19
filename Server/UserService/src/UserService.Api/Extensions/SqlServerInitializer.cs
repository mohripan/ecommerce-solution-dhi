using Microsoft.Data.SqlClient;

namespace UserService.Api.Extensions
{
    public static class SqlServerInitializer
    {
        public static void EnsureDatabaseAndTables(string connectionString)
        {
            var builder = new SqlConnectionStringBuilder(connectionString);
            var targetDatabase = builder.InitialCatalog;

            var masterConnString = $"Server={builder.DataSource};Database=master;User Id={builder.UserID};Password={builder.Password};Encrypt=False;";

            using (var masterConnection = new SqlConnection(masterConnString))
            {
                masterConnection.Open();
                var createDbCmd = masterConnection.CreateCommand();
                createDbCmd.CommandText = $@"
                IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'{targetDatabase}')
                BEGIN
                    PRINT 'Creating database [{targetDatabase}]...';
                    CREATE DATABASE [{targetDatabase}];
                END;
            ";
                createDbCmd.ExecuteNonQuery();
            }

            using (var userDbConnection = new SqlConnection(connectionString))
            {
                userDbConnection.Open();

                var createMstrRoleTableSql = @"
                IF NOT EXISTS (SELECT * FROM sys.objects 
                               WHERE object_id = OBJECT_ID(N'[dbo].[MstrRole]') 
                               AND type in (N'U'))
                BEGIN
                    CREATE TABLE [dbo].[MstrRole](
	                    [Id] INT NOT NULL IDENTITY(1,1),
	                    [RoleName] NVARCHAR(100) NOT NULL,
	                    [CreatedOn] DATETIME2 NOT NULL DEFAULT (GETUTCDATE()),
	                    [CreatedBy] NVARCHAR(50) NOT NULL DEFAULT ('SYS'),
	                    [ModifiedOn] DATETIME2 NULL,
	                    [ModifiedBy] NVARCHAR(50) NULL,
	                    CONSTRAINT [PK_MstrRole] PRIMARY KEY CLUSTERED ([Id] ASC)
                    );
                END;
            ";
                var createMstrUserTableSql = @"
                IF NOT EXISTS (SELECT * FROM sys.objects 
                               WHERE object_id = OBJECT_ID(N'[dbo].[MstrUser]') 
                               AND type in (N'U'))
                BEGIN
                    CREATE TABLE [dbo].[MstrUser](
	                    [Id] INT NOT NULL IDENTITY(1,1),
	                    [Username] NVARCHAR(100) NOT NULL,
	                    [Email] NVARCHAR(100) NOT NULL,
	                    [Password] NVARCHAR(200) NOT NULL,
	                    [RoleId] INT NOT NULL,
	                    [SecurityStamp] NVARCHAR(200) NOT NULL,
	                    [CreatedOn] DATETIME2 NOT NULL DEFAULT (GETUTCDATE()),
	                    [CreatedBy] NVARCHAR(50) NOT NULL DEFAULT ('SYS'),
	                    [ModifiedOn] DATETIME2 NULL,
	                    [ModifiedBy] NVARCHAR(50) NULL,
	                    CONSTRAINT [PK_MstrUser] PRIMARY KEY CLUSTERED ([Id] ASC),
	                    CONSTRAINT [FK_MstrUser_MstrRole] FOREIGN KEY ([RoleId])
	                        REFERENCES [dbo].[MstrRole] ([Id]) 
	                        ON DELETE NO ACTION
                    );
                END;
            ";

                using (var cmd = userDbConnection.CreateCommand())
                {
                    cmd.CommandText = createMstrRoleTableSql;
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = createMstrUserTableSql;
                    cmd.ExecuteNonQuery();
                }

                var seedRolesSql = @"
                IF NOT EXISTS (SELECT 1 FROM [dbo].[MstrRole] WHERE [Id] = 1)
                BEGIN
                    INSERT INTO [dbo].[MstrRole] ([RoleName], [CreatedOn]) 
                    VALUES ('Buyer', GETUTCDATE());
                END;

                IF NOT EXISTS (SELECT 1 FROM [dbo].[MstrRole] WHERE [Id] = 2)
                BEGIN
                    INSERT INTO [dbo].[MstrRole] ([RoleName], [CreatedOn]) 
                    VALUES ('Seller', GETUTCDATE());
                END;
            ";

                using (var cmd = userDbConnection.CreateCommand())
                {
                    cmd.CommandText = seedRolesSql;
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
