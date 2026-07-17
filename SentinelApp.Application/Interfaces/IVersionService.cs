using SentinelApp.Application.Core;
using SentinelApp.Application.DTO;
using System.Threading.Tasks;

namespace SentinelApp.Application.Interfaces
{
    public interface IVersionService
    {
        Task<ServiceResponse<AppVersionResponse>> CheckVersionAsync(AppVersionView appVersionView);
    }
}