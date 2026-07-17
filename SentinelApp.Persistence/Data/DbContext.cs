using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
namespace SentinelApp.Persistence.Data
{
    public interface IDbContext
    {
        IDbConnection CreateConnection();
    }
    public class DbContext : IDbContext
    {
        private readonly IConfiguration _configuration;

        public DbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        }
    }
}
