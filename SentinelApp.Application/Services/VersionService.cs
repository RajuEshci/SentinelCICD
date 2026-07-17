using Microsoft.Extensions.Logging;
using SentinelApp.Application.Core;
using SentinelApp.Application.DTO;
using SentinelApp.Application.Interfaces;
using SentinelApp.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SentinelApp.Application.Services
{
    public class VersionService : IVersionService
    {
        private readonly IVersionRepository _versionRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<VersionService> _logger;

        public VersionService(IVersionRepository versionRepository, ICurrentUserService currentUserService, ILogger<VersionService> logger)
        {
            _versionRepository = versionRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<ServiceResponse<AppVersionResponse>> CheckVersionAsync(AppVersionView appVersionView)
        {
            return await ServiceResponseExceptionHandler.Handle<AppVersionResponse>(async () =>
            {
                var languageId = _currentUserService.PreferredLanguageId;
                if (languageId == 0)
                    languageId = 1;

                var versions = await _versionRepository.GetVersionsGreaterThanAsync(appVersionView.Version, languageId);

                var response = new AppVersionResponse();

                if (versions.Count == 0)
                {
                    response.LatestVersion = appVersionView.Version;
                }

                foreach (var version in versions)
                {
                    response.LatestVersion = version.VersionNo;
                    if (!response.UpdateRequired)
                    {
                        response.UpdateRequired = version.IsForcefullyUpdate;
                    }
                    response.Messages.Add(new AppVersionMessages { Messages = version.Message ?? string.Empty });
                }

                return response;
            }, "Version check completed.");
        }
    }
}