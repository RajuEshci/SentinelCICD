using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SentinelApp.Application.Core;
using SentinelApp.Application.DTO;
using SentinelApp.Application.Interfaces;
using Newtonsoft.Json;
using SentinelApp.API.Middleware;

namespace SentinelApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InformativePagesController : ControllerBaseResponse
    {
        private readonly IInformativeService _informativeService;
        private readonly ILogger<InformativePagesController> _logger;
		private readonly ICurrentUserService _currentUserService;

		public InformativePagesController(IInformativeService informativeService, ILogger<InformativePagesController> logger, ICurrentUserService currentUserService)
        {
            _informativeService = informativeService;
            _logger = logger;
            _currentUserService = currentUserService;
        }

        [AllowAnonymous]
        [HttpPost("getbypagecodeid")]
        public async Task<IActionResult> GetByPageCodeIdAsync(InformativePagesRequestDto objRequestData)
        {
            return GenerateBaseResponse(await _informativeService.GetByPageCodeIdAsync(objRequestData));
        }

		[HttpGet("getmenulist")]
		public async Task<IActionResult> GetMenuList()
		{
			return GenerateBaseResponse(await _informativeService.GetMenuListAsync());
		}
	}
}
