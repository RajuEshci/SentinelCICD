using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SentinelApp.Application.Core;
using SentinelApp.Application.DTO;
using SentinelApp.Application.Interfaces;

namespace SentinelApp.API.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class AppVersionController : ControllerBaseResponse
    {
        private readonly IVersionService _versionService;
        private readonly ILogger<AppVersionController> _logger;

        public AppVersionController(IVersionService versionService, ILogger<AppVersionController> logger)
        {
            _versionService = versionService;
            _logger = logger;
        }

        [HttpPost("AppVersion")]
        [AllowAnonymous]
        public async Task<IActionResult> AppVersion(AppVersionView appVersionView)
        {
            return GenerateBaseResponse(await _versionService.CheckVersionAsync(appVersionView));
        }
    }
}