using DbUp;
using SentinelApp.Persistence;

namespace SentinelApp.Infrastructure.Database
{
    public static class DatabaseMigration
    {
        public static void Run(string connectionString, bool dryRun)
        {
            if (dryRun)
            {
                RunDryRun(connectionString);
            }
            else
            {
                RunActualMigration(connectionString);
            }
        }

        private static void RunActualMigration(string connectionString)
        {
            var upgrader =
                DeployChanges.To
                    .SqlDatabase(connectionString)
                    .WithScriptsEmbeddedInAssembly(
                        typeof(MigrationMarker).Assembly) 
                    .WithTransaction()
                    .LogToConsole()
                    .Build();

            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                throw result.Error;
            }
        }

        private static void RunDryRun(string connectionString)
        {
            using var connection = new System.Data.SqlClient.SqlConnection(connectionString);
            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                var upgrader =
                    DeployChanges.To
                        .SqlDatabase(connectionString)
                        .WithScriptsEmbeddedInAssembly(
                            typeof(MigrationMarker).Assembly)
                        .WithTransaction()
                        .LogToConsole()
                        .Build();

                var result = upgrader.PerformUpgrade();

                if (!result.Successful)
                {
                    throw result.Error;
                }

                // 🧪 Always rollback in dry-run
                transaction.Rollback();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
