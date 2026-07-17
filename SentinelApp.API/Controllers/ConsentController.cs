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
    public class ConsentController : ControllerBaseResponse
    {
        private readonly IConsentService _consentService;
        private readonly ILogger<ConsentController> _logger;
		private readonly ICurrentUserService _currentUserService;

		public ConsentController(IConsentService consentService, ILogger<ConsentController> logger, ICurrentUserService currentUserService)
        {
			_consentService = consentService;
            _logger = logger;
            _currentUserService = currentUserService;
        }

        [HttpGet("getconsentbyuserid")]
        public async Task<IActionResult> GetConsentByUserId()
        {
			return GenerateBaseResponse(await _consentService.GetConsentByUserId());
        }

		[HttpPost("updateconsent")]
		public async Task<IActionResult> UpdateConsent(List<ConsentUserRequestDto> userConsentAnswerDto)
		{
			return GenerateBaseResponse(await _consentService.UpdateConsentAnswerAsync(userConsentAnswerDto));
		}
	}
}
