using Microsoft.Data.SqlClient;
using System;
using System.Data.SqlClient;
using System.IO;
using DbUp;
using SqlConnectionStringBuilder = System.Data.SqlClient.SqlConnectionStringBuilder;
using SqlConnection = System.Data.SqlClient.SqlConnection;
using SqlCommand = System.Data.SqlClient.SqlCommand;

namespace SentinelApp.Infrastructure.Database
{
    public static class DatabaseBackupService
    {
        public static void BackupDatabase(
            string connectionString,
            string backupDirectory)
        {
            if (string.IsNullOrWhiteSpace(backupDirectory))
                throw new ArgumentException(
                    "Backup directory path is not configured.",
                    nameof(backupDirectory));

            Directory.CreateDirectory(backupDirectory);

            var builder = new SqlConnectionStringBuilder(connectionString);
            var databaseName = builder.InitialCatalog;

            var backupFileName =
                $"{databaseName}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.bak";

            var backupFilePath =
                Path.Combine(backupDirectory, backupFileName);

            var backupSql = $@"BACKUP DATABASE [{databaseName}]
                            TO DISK = N'{backupFilePath}'
                            WITH INIT, FORMAT;
                            ";

            using var connection = new SqlConnection(connectionString);
            connection.Open();

            using var command = new SqlCommand(backupSql, connection)
            {
                CommandTimeout = 0 // Important for large databases
            };

            command.ExecuteNonQuery();
        }
    }
}
