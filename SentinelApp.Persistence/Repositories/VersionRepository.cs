using Dapper;
using SentinelApp.Domain.Entities;
using SentinelApp.Domain.Interfaces;
using SentinelApp.Persistence.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentinelApp.Persistence.Repositories
{
    public class VersionRepository : IVersionRepository
    {
        private readonly IDbContext _dbContext;

        public VersionRepository(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<AppVersionEntity>> GetVersionsGreaterThanAsync(string version, int languageId)
        {
            using var _connection = _dbContext.CreateConnection();
            const string sql = @"select v.Id, v.VersionNo, v.IsForcefullyUpdate, vm.Message
from Versions v
left join VersionMessages vm on vm.VersionId = v.Id and vm.LanguageId = @languageId
where v.VersionNo > @version
order by v.VersionNo";
            return (await _connection.QueryAsync<AppVersionEntity>(sql, new { version, languageId })).ToList();
        }
    }
}