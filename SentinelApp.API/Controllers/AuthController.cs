using Azure;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SentinelApp.API.Middleware;
using SentinelApp.Application.Core;
using SentinelApp.Application.DTO;
using SentinelApp.Application.DTO.Response;
using SentinelApp.Application.Interfaces;
using System.Security.Claims;

namespace SentinelApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBaseResponse
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;
		private readonly ICurrentUserService _currentUserService;

		public AuthController(IAuthService authService, ILogger<AuthController> logger, ICurrentUserService currentUserService)
        {
            _authService = authService;
            _logger = logger;
            _currentUserService = currentUserService;
        }

        [HttpPost("register")]
		[AllowAnonymous]
        public async Task<IActionResult> Register(RegisterDto objRequestData)
        {
			return GenerateBaseResponse(await _authService.CreateUser(objRequestData));
        }

        [HttpPost("getuserbyid")]
        public async Task<IActionResult> GetUserById(UserDto objRequestData)
        {
			return GenerateBaseResponse(await _authService.GetByUserId(objRequestData.UserId));
        }
        [HttpDelete("deleteuser")]
        public async Task<IActionResult> DeleteByUserId()
        {
			return GenerateBaseResponse(await _authService.DeleteById());
        }
        [HttpPost("updateUser")]
        public async Task<IActionResult> updateUser(UpdateUserDto objRequestData)
        {
			return GenerateBaseResponse(await _authService.UpdateUser(objRequestData));
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginDto objRequestData)
        {
			return GenerateBaseResponse(await _authService.LoginAsync(objRequestData));
        }

        [HttpPost("forgotPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto objRequestData)
        {
			return GenerateBaseResponse(await _authService.ForgotPasswordAsync(objRequestData.EmailId));
        }

		
		[HttpGet("getUser")]
		public async Task<IActionResult> GetUser()
		{
            var userId = int.TryParse(_currentUserService.UserId, out var id) ? id : 0;
			return GenerateBaseResponse(await _authService.GetByUserId(id));
		}

		[HttpPost("verifyOtp")]
		[AllowAnonymous]
		public async Task<IActionResult> VerifyOtp(VerifyOtpDto objRequestData)
		{
			return GenerateBaseResponse(await _authService.VerifyOtp(objRequestData));
		}

		[HttpPost("resendOtp")]
		[AllowAnonymous]
		public async Task<IActionResult> resendOtp(VerifyOtpDto objRequestData)
		{
			return GenerateBaseResponse(await _authService.ResendOtp(objRequestData));
		}

		[HttpPost("validateaccesscode")]
		[AllowAnonymous]
		public async Task<IActionResult> ValidateAccessCode(ValidateAccessCodeDto objRequestData)
		{
			return GenerateBaseResponse(await _authService.ValidateAccessCode(objRequestData.AccessCode));
		}

		[HttpPost("addaccesscodeemail")]
		[AllowAnonymous]
		public async Task<IActionResult> AddAccessCodeEmail(AccessCodeEmailDto objRequestData)
		{
			return GenerateBaseResponse(await _authService.AddAccessCodeEmailAsync(objRequestData));
		}

		[HttpPost("verifyaccesscodeotp")]
		[AllowAnonymous]
		public async Task<IActionResult> VerifyAccessCodeOtp(VerifyAccessCodeOtp objRequestData)
		{
			return GenerateBaseResponse(await _authService.VerifyAccessCodeOtpAsync(objRequestData));
		}

		[HttpPost("requestaccesscode")]
		[AllowAnonymous]
		public async Task<IActionResult> RequestAccessCode(UserAccessCodeRequestDto objRequestData)
		{
			return GenerateBaseResponse(await _authService.RequestAccessCodeAsync(objRequestData));
		}

		[HttpPost("verifyrequestaccesscodeotp")]
		[AllowAnonymous]
		public async Task<IActionResult> VerifyRequestAccessCodeOtp(VerifyAccessCodeOtp objRequestData)
		{
			return GenerateBaseResponse(await _authService.VerifyRequestAccessCodeOtpAsync(objRequestData));
		}

		[HttpGet("GenerateAccessCodeFromEmail")]
		[AllowAnonymous]
		public async Task<IActionResult> GenerateAccessCodeFromEmail([FromQuery] AccessCodeRequestDto objRequestData)
		{
			return GenerateBaseResponse(await _authService.GenerateAccessCodeFromEmailAsync(objRequestData));
        }

		[HttpPost("updateuserpassword")]
		public async Task<IActionResult> UpdateUserPassword(ChangePasswordDto changePasswordDto)
		{
			return GenerateBaseResponse(await _authService.UpdateUserPasswordAsync(changePasswordDto));
		}

		[HttpPut("disableuser")]
		public async Task<IActionResult> DisableUser()
		{
			return GenerateBaseResponse(await _authService.DisableUser());
		}

		[AllowAnonymous]
		[HttpPost("validatetoken")]
		public async Task<IActionResult> ValidateToken(TokenRequestDto tokenRequestDto)
		{
			return GenerateBaseResponse(await _authService.ValidateTokenAsync(tokenRequestDto));
		}
	}
}
