using MySqlConnector;

namespace ProductService.Api.Extensions
{
    public static class MySqlInitializer
    {
        public static void EnsureDatabaseAndTables(string connectionString)
        {
            var builder = new MySqlConnectionStringBuilder(connectionString);
            var targetDatabase = builder.Database;
            var sysConnString = $"Server={builder.Server};Database=mysql;Uid={builder.UserID};Pwd={builder.Password}";
            using (var sysConnection = new MySqlConnection(sysConnString))
            {
                sysConnection.Open();
                var createDbCmd = sysConnection.CreateCommand();
                createDbCmd.CommandText = $@"
                CREATE DATABASE IF NOT EXISTS `{targetDatabase}`;
            ";
                createDbCmd.ExecuteNonQuery();
            }

            var targetConnString = connectionString;
            using (var productConn = new MySqlConnection(targetConnString))
            {
                productConn.Open();

                var createMstrCategorySql = @"
                CREATE TABLE IF NOT EXISTS `MstrCategory`(
                    `Id` INT NOT NULL AUTO_INCREMENT,
                    `CategoryName` VARCHAR(100) NOT NULL,
                    `CreatedOn` DATETIME NOT NULL DEFAULT '2025-01-01',
                    `CreatedBy` VARCHAR(50) NOT NULL DEFAULT 'SYS',
                    `ModifiedOn` DATETIME NULL,
                    `ModifiedBy` VARCHAR(50) NULL,
                    PRIMARY KEY(`Id`)
                );
            ";
                var createProductSql = @"
                CREATE TABLE IF NOT EXISTS `Product`(
                    `Id` INT NOT NULL AUTO_INCREMENT,
                    `CategoryId` INT NOT NULL,
                    `Name` VARCHAR(100) NOT NULL,
                    `UserId` INT NOT NULL,
                    `Quantity` INT NOT NULL DEFAULT 0,
                    `Price` DOUBLE NOT NULL,
                    `CreatedOn` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    `CreatedBy` INT NOT NULL,
                    `ModifiedOn` DATETIME NULL,
                    `ModifiedBy` VARCHAR(50) NULL,
                    PRIMARY KEY(`Id`),
                    CONSTRAINT `FK_Product_Category` FOREIGN KEY (`CategoryId`) 
                        REFERENCES `MstrCategory` (`Id`) ON DELETE RESTRICT
                );
            ";

                // Execute table creation
                using (var cmd = productConn.CreateCommand())
                {
                    cmd.CommandText = createMstrCategorySql;
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = createProductSql;
                    cmd.ExecuteNonQuery();
                }

                // 3) Insert seed data if missing
                var seedCategoriesSql = @"
                INSERT INTO `MstrCategory` (Id, CategoryName)
                SELECT 1, 'Technology'
                FROM DUAL
                WHERE NOT EXISTS (SELECT 1 FROM `MstrCategory` WHERE `Id` = 1);

                INSERT INTO `MstrCategory` (Id, CategoryName)
                SELECT 2, 'Fashion'
                FROM DUAL
                WHERE NOT EXISTS (SELECT 1 FROM `MstrCategory` WHERE `Id` = 2);

                INSERT INTO `MstrCategory` (Id, CategoryName)
                SELECT 3, 'Food & Drink'
                FROM DUAL
                WHERE NOT EXISTS (SELECT 1 FROM `MstrCategory` WHERE `Id` = 3);

                INSERT INTO `MstrCategory` (Id, CategoryName)
                SELECT 4, 'Others'
                FROM DUAL
                WHERE NOT EXISTS (SELECT 1 FROM `MstrCategory` WHERE `Id` = 4);
            ";

                using (var cmd = productConn.CreateCommand())
                {
                    cmd.CommandText = seedCategoriesSql;
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
