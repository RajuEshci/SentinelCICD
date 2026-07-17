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
    public class MedicalHistoryController : ControllerBaseResponse
    {
        private readonly IMedicalHistoryService _medicalHistoryService;
        private readonly ILogger<MedicalHistoryController> _logger;
		private readonly ICurrentUserService _currentUserService;

		public MedicalHistoryController(IMedicalHistoryService medicalHistoryService, ILogger<MedicalHistoryController> logger, ICurrentUserService currentUserService)
        {
            _medicalHistoryService = medicalHistoryService;
            _logger = logger;
            _currentUserService = currentUserService;
        }

		[HttpGet("getmedicalhistory")]
		public async Task<IActionResult> GetMedicalHistory()
		{
			return GenerateBaseResponse(await _medicalHistoryService.GetQuestionnaireAsync());
		}

		[HttpPost("upsertuseranswers")]
		public async Task<IActionResult> UpsertUserAnswers(UpsertUserAnswerDto createUserAnswerDto)
		{
			return GenerateBaseResponse(await _medicalHistoryService.UpsertUserAnswer(createUserAnswerDto));
		}

		[HttpGet("getuseranswers")]
		public async Task<IActionResult> GetuserAnswers()
		{
			return GenerateBaseResponse(await _medicalHistoryService.GetUserAnswersAsync());
		}

		[HttpPost("getuseranswerhistory")]
		public async Task<IActionResult> GetUserAnswerHistory(UserAnswerHistoryFilterDto userAnswerHistoryFilterDto)
		{
			return GenerateBaseResponse(await _medicalHistoryService.GetMedicalHistoryAsync(userAnswerHistoryFilterDto));
		}

		[HttpGet("getactions")]
		public async Task<IActionResult> GetActions()
		{
			return GenerateBaseResponse(await _medicalHistoryService.GetActionsByUserId());
		}

		[HttpGet("getreport")]
		public async Task<IActionResult> GetReport()
		{
			return GenerateBaseResponse(await _medicalHistoryService.GenerateMedicalHistoryPdf());
		}
	}
}
