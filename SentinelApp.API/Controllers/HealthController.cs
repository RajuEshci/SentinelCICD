using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SentinelApp.Application.Core;
using SentinelApp.Application.Interfaces;
using System.Diagnostics;

namespace SentinelApp.API.Controllers
{
    [Route("api/health")]
    [ApiController]
    public class HealthController : ControllerBaseResponse
    {
        private readonly ILogger<HealthController> _logger;
        private readonly IAuthService _authService;
        public HealthController(ILogger<HealthController> logger, IAuthService authService)
        {
            _logger = logger;
            _authService = authService;
        }

        [HttpGet("ping")]
        [AllowAnonymous]
        public IActionResult Ping()
        {
            return Ok(new { status = "Healthy", timestampUtc = DateTime.UtcNow });
        }

        [HttpGet("status")]
        [AllowAnonymous]
        public async Task<IActionResult> Status()
        {
            return GenerateBaseResponse(await _authService.DBHealthy());
        }
    }
}
