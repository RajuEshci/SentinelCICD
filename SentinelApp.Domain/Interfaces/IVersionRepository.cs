using SentinelApp.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SentinelApp.Domain.Interfaces
{
    public interface IVersionRepository
    {
        Task<List<AppVersionEntity>> GetVersionsGreaterThanAsync(string version, int languageId);
    }
}