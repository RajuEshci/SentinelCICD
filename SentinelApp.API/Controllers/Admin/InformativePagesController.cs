using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SentinelApp.Application.Core;
using SentinelApp.Application.DTO;
using SentinelApp.Application.Interfaces;
using Newtonsoft.Json;
using SentinelApp.API.Middleware;

namespace SentinelApp.API.Controllers.Admin
{
	[Route("api/admin/[controller]")]
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

		[HttpPost("getallpages")]
		public async Task<IActionResult> GetAllPages(InformativePagesFilterDto informativePagesFilterDto)
		{
			return GenerateBaseResponse(await _informativeService.GetAllPagesAsync(informativePagesFilterDto));
		}

		[HttpPost("createinformativepages")]
		public async Task<IActionResult> CreateInformativePages(CreateInformativePagesDto createInformativePagesDto)
		{
			return GenerateBaseResponse(await _informativeService.CreateInformativePagesAsync(createInformativePagesDto));
		}

		[HttpPost("updateinformativepages/{id}")]
		public async Task<IActionResult> UpdateInformativePages(int id, UpdateInformativePagesDto updateInformativePagesDto)
		{
			return GenerateBaseResponse(await _informativeService.UpdatePagesAsync(id, updateInformativePagesDto));
		}

		[HttpDelete("deleteinformativepages/{id}")]
		public async Task<IActionResult> DeleteInformativePages(int id)
		{
			return GenerateBaseResponse(await _informativeService.DeletePageAsync(id));
		}

		[HttpGet("getallpageslist")]
		public async Task<IActionResult> GetAllPagesList()
		{
			return GenerateBaseResponse(await _informativeService.GetAllPagesListAsync());
		}
	}
}
