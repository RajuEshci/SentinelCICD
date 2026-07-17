using Azure.Core;
using BCrypt.Net;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Scriban;
using SentinelApp.Application.Auth;
using SentinelApp.Application.Core;
using SentinelApp.Application.Core.CommonExtension;
using SentinelApp.Application.DTO;
using SentinelApp.Application.DTO.Response;
using SentinelApp.Application.Interfaces;
using SentinelApp.Domain.Entities;
using SentinelApp.Domain.Interfaces;
using SentinelApp.Persistence.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Web;

namespace SentinelApp.Infrastructure
{
	public class AuthService : IAuthService
	{
		private readonly IUserRepository _users;
		private readonly IConfiguration _config;
		private readonly ICurrentUserService _currentUserService;
		private readonly ICheckDuplicateRepository _checkDuplicateRepository;
		private readonly int _userId;
		private readonly Email _email;
		private readonly IEncryptionService _encryptionService;
		private readonly IValidator<RegisterDto> _validator;
		private readonly IValidator<UpdateUserDto> _updateUserValidator;
		private readonly int _languageId;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public AuthService(IUserRepository users, IConfiguration config, ICurrentUserService currentUserService, ICheckDuplicateRepository checkDuplicateRepository, Email email, IEncryptionService encryptionService, IValidator<RegisterDto> validator, IValidator<UpdateUserDto> updateUserValidator, IHttpContextAccessor httpContextAccessor)
		{
			_users = users;
			_config = config;
			_currentUserService = currentUserService;
			_checkDuplicateRepository = checkDuplicateRepository;
			_userId = int.TryParse(_currentUserService.UserId, out var id) ? id : 0;
			_email = email;
			_encryptionService = encryptionService;
			_validator = validator;
			_updateUserValidator = updateUserValidator;
			_languageId = _currentUserService.PreferredLanguageId;
			_httpContextAccessor = httpContextAccessor;
		}

		private async Task<string> GenerateJwtToken(User user)
		{
			var userJson = JsonSerializer.Serialize(user);
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.Sid, user.UserId.ToString()),
				new Claim(ClaimTypes.Name, user.UserName ?? ""),
				new Claim(ClaimTypes.Email, user.EmailId ?? ""),
				new Claim("UserObject", userJson)
			};

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				issuer: _config["Jwt:Issuer"],
				audience: _config["Jwt:Audience"],
				claims: claims,
				expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["Jwt:ExpiresInMinutes"])),
				signingCredentials: creds);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}



		async Task<ServiceResponse<User?>> IAuthService.LoginAsync(LoginDto dto)
		{
			return await ServiceResponseExceptionHandler.Handle<User?>(async () =>
			{
				var user = await _users.ValidateLogin(dto.EmailId);
				if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
					throw new ServiceResponseException(System.Net.HttpStatusCode.Conflict, "Either username or password is incorrect!");

				if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
					throw new ServiceResponseException(System.Net.HttpStatusCode.Conflict, "User name or password not match.");

				if (user?.IsExported == true)
					throw new ServiceResponseException(System.Net.HttpStatusCode.Conflict, "You need to re-register in the application. Please follow the registration process.");

				else if (user?.IsActive == false)
					throw new ServiceResponseException(System.Net.HttpStatusCode.Conflict, "User is disabled. Please contact the administrator");

				else if (user?.IsOtpVerify == false)
					throw new ServiceResponseException(System.Net.HttpStatusCode.Conflict, "Please verify your account. OTP is sent to your registered email id.");

				#region refresh token
				var refreshToken = Guid.NewGuid().ToString();
				DateTime? expireAt = DateTime.UtcNow.AddMinutes(5);
				var RefreshTokenExpiresDays = Convert.ToDouble(_config["Jwt:RefreshTokenValidityInDays"]);
				DateTime RefreshTokenExpiresAt = expireAt ?? DateTime.UtcNow.AddDays(RefreshTokenExpiresDays);
				DateTime RevokedAt = DateTime.UtcNow;
				var refreshTokenData = new RefreshTokenMaster
				{
					UserId = user.UserId,
					Token = refreshToken,
					CreatedByIp = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString(),
					ExpiresAt = RefreshTokenExpiresAt,
					RevokedAt = RevokedAt,
				};
				await _users.GenerateRefreshToken(refreshTokenData);
				#endregion
				user.Token = await GenerateJwtToken(user);
				user.RefreshToken = refreshToken;
				user.Password = string.Empty;
				return user;
			}, "Login Successfully.");
		}

		public async Task<ServiceResponse<bool?>> DBHealthy()
		{
            return await ServiceResponseExceptionHandler.Handle<bool?>(async () =>
            {
                var result = await _users.DBHealthy();
                if (result == null)
                    throw new ServiceResponseException(HttpStatusCode.ServiceUnavailable, "Database is not healthy.");
                return result;
            }, "Database is healthy. ");
        }

        public async Task<ServiceResponse<bool?>> CreateUser(RegisterDto dto)
		{
			string successMsg = dto.AccessCode.Contains("play", StringComparison.OrdinalIgnoreCase) ? "User created successfully. OTP is sent to your registered email id for verification." : "User created successfully.";
			return await ServiceResponseExceptionHandler.Handle<bool?>(async () =>
			{
				var result = await _validator.ValidateAsync(dto);

				if (!result.IsValid)
				{
					throw new ServiceResponseException(System.Net.HttpStatusCode.BadRequest, result.Errors.First().ErrorMessage);
				}
				var duplicateMessage = await _checkDuplicateRepository.CheckDuplicateAsync("Users", new Dictionary<string, object?>
				{
					{ "Mobilenumber", dto.MobileNumber },
					{ "PhoneNumber", dto.PhoneNumber },
					{ "EmailId", dto.EmailId },
				});

				if (duplicateMessage != null)
				{
					throw new ServiceResponseException(System.Net.HttpStatusCode.Conflict, duplicateMessage);
				}
				var accessCode = await _users.GetAccessCodeAsync(dto.AccessCode);
				if (accessCode == null)
					throw new ServiceResponseException(System.Net.HttpStatusCode.Conflict, "Invalid Access Code.");
				if (!accessCode.AssignedTo.Equals(dto.EmailId, StringComparison.OrdinalIgnoreCase))
				{
					throw new ServiceResponseException(HttpStatusCode.Conflict, "The email-id does not match our records.");
				}
				var lang = await _users.GetLanguageFromAccessCodeAsync(dto.AccessCode);
				var otp = new Random().Next(100000, 999999).ToString();
				var user = new User
				{
					UserName = dto.UserName,
					Gender = dto.Gender,
					DateOfBirth = dto.DateOfBirth,
					PhoneNumber = dto.PhoneNumber,
					MobileNumber = dto.MobileNumber,
					EmailId = dto.EmailId,
					Address = dto.Address,
					PostCode = dto.PostCode,
					Country = dto.Country,
					Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
					IsVerify = false,
					Otp = otp,
					OtpGenOn = DateTime.UtcNow,
					AccessCode = dto.AccessCode,
					IsOtpVerify = false,
					PreferredLanguageCode = lang != null ? lang.LanguageCode : "en-GB",
					PreferredLanguageId = lang != null ? lang.LanguageId : 1,
					CreatedBy = _userId,
					IsConsentUpdated = false,
					OtpAttempts = 3,
					QuestionnaireId = dto.QuestionnaireId,
				};
				var userid = await _users.CreateAsync(user);
				if (dto.AccessCode.Contains("play", StringComparison.OrdinalIgnoreCase))
				{
					var htmlTemplate = LoadOtpHtmlTemplate(user.PreferredLanguageCode);
					var template = Template.Parse(htmlTemplate);
					var renderedHtml = template.Render(new { otp = user.Otp, year = DateTime.Now.Year });
					_email.SendOtpMail(dto.EmailId, renderedHtml);
				}
				else
				{
					var userData = new User
					{
						IsOtpVerify = true,
						IsActive = true,
						IsVerify = true,
						UpdatedBy = userid,
						UserId = userid,
					};
					await _users.VerifyOtpAsync(userData);
				}
				return true;
			}, successMsg);
		}

		public async Task<ServiceResponse<bool?>> UpdateUser(UpdateUserDto dto)
		{
			return await ServiceResponseExceptionHandler.Handle<bool?>(async () =>
			{
				var result = await _updateUserValidator.ValidateAsync(dto);

				if (!result.IsValid)
				{
					throw new ServiceResponseException(System.Net.HttpStatusCode.BadRequest, result.Errors.First().ErrorMessage);
				}
				var duplicateMessage = await _checkDuplicateRepository.CheckDuplicateAsync("Users", new Dictionary<string, object?>
				{
					{ "Mobilenumber", dto.MobileNumber },
					{ "PhoneNumber", dto.PhoneNumber },
					{ "EmailId", dto.EmailId }
				}, excludeIdColumn: "UserId", excludeIdValue: _userId);

				if (duplicateMessage != null)
				{
					throw new ServiceResponseException(System.Net.HttpStatusCode.Conflict, duplicateMessage);
				}
				var user = await _users.GetByIdAsync(_userId);
				if (user == null)
					throw new ServiceResponseException(System.Net.HttpStatusCode.Conflict, "User not found.");
				if (!string.IsNullOrEmpty(dto.UserName))
				{
					user.UserName = dto.UserName;
				}
				if (!string.IsNullOrEmpty(dto.Gender))
				{
					user.Gender = dto.Gender;
				}
				if (dto.DateOfBirth.HasValue)
				{
					user.DateOfBirth = dto.DateOfBirth.Value;
				}
				if (!string.IsNullOrEmpty(dto.Address))
				{
					user.Address = dto.Address;
				}
				if (!string.IsNullOrEmpty(dto.PostCode))
				{
					user.PostCode = dto.PostCode;
				}
				if (!string.IsNullOrEmpty(dto.Country))
				{
					user.Country = dto.Country;
				}
				if (!string.IsNullOrEmpty(dto.PhoneNumber))
				{
					user.PhoneNumber = dto.PhoneNumber;
				}
				if (!string.IsNullOrEmpty(dto.MobileNumber))
				{
					user.MobileNumber = dto.MobileNumber;
				}
				if (!string.IsNullOrEmpty(dto.EmailId))
				{
					user.EmailId = dto.EmailId;
				}
				await _users.UpdateAsync(user);
				return true;
			}, "User updated successfully.");
		}

		public async Task<ServiceResponse<UserDto>> GetByUserId(int Id)
		{
			return await ServiceResponseExceptionHandler.Handle<UserDto>(async () =>
			{
				if (Id == 0)
					throw new ServiceResponseException(System.Net.HttpStatusCode.Conflict, "User Not Found");

				var data = await _users.GetByIdAsync(Id);
				return data != null ? MapToDto(data) : null;
			}, "Data retrieved successfully.");
		}

		public async Task<ServiceResponse<bool>> DeleteById()
		{
			return await ServiceResponseExceptionHandler.Handle<bool>(async () =>
			{
				await _users.DeleteAsync(_userId);
				return true;
			}, "User deleted successfully.");
		}
		public async Task<ServiceResponse<bool>> DisableUser()
		{
			return await ServiceResponseExceptionHandler.Handle<bool>(async () =>
			{
				var user = await _users.GetByIdAsync(_userId);
				if (user == null)
					throw new ServiceResponseException(System.Net.HttpStatusCode.Conflict, "User Not Found");
				await _users.DisableUserAsync(_userId);
				return true;
			}, "User deleted successfully.");
		}

		public async Task<ServiceResponse<bool>> ForgotPasswordAsync(string email)
		{
			return await ServiceResponseExceptionHandler.Handle<bool>(async () =>
			{
				var user = await _users.GetByEmailAsync(email, 0);
				if (user == null)
					throw new ServiceResponseException(System.Net.HttpStatusCode.Conflict, "User Not Found");
				var newPassword = CommonExtension.GeneratePassword(8, true, true, true, true);
				user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
				user.UpdatedOn = DateTime.UtcNow;
				await _users.UpdateAsync(user);
				var htmlTemplate = LoadHtmlTemplate(user.PreferredLanguageCode);
				var template = Template.Parse(htmlTemplate);
				var renderedHtml = template.Render(new { username = user.UserName, alink = newPassword, year = DateTime.Now.Year });
				_email.SendEmailOnForgotPassword(email, renderedHtml);
				return true;
			}, "New password is generated and sent on your registered email id.");
		}
		public async Task<ServiceResponse<bool>> VerifyOtp(VerifyOtpDto verifyOtpDto)
		{
			return await ServiceResponseExceptionHandler.Handle<bool>(async () =>
			{
				var user = await _users.GetByEmailAsync(verifyOtpDto.EmailId, 0);
				if (user == null)
					throw new ServiceResponseException(System.Net.HttpStatusCode.OK, "User Not Found.");
				user.OtpAttempts--;
				if (user.Otp != verifyOtpDto.Otp)
				{
					throw new ServiceResponseException(System.Net.HttpStatusCode.OK, "Invalid Otp.");
				}
				user.IsVerify = true;
				user.IsActive = true;
				user.IsOtpVerify = true;
				user.UpdatedBy = _userId;
				await _users.VerifyOtpAsync(user);
				return true;
			}, "OTP verified successfully.");
		}

		public async Task<ServiceResponse<bool>> ResendOtp(VerifyOtpDto verifyOtpDto)
		{
			return await ServiceResponseExceptionHandler.Handle<bool>(async () =>
			{
				var user = await _users.GetByEmailAsync(verifyOtpDto.EmailId, 0);
				if (user == null)
					throw new ServiceResponseException(System.Net.HttpStatusCode.OK, "User Not Found.");

				user.Otp = new Random().Next(100000, 999999).ToString();
				user.UpdatedBy = _userId;
				await _users.UpdateOtpAsync(user);
				var htmlTemplate = LoadOtpHtmlTemplate(user.PreferredLanguageCode);
				var template = Template.Parse(htmlTemplate);
				var renderedHtml = template.Render(new { otp = user.Otp, year = DateTime.Now.Year });
				_email.SendOtpMail(verifyOtpDto.EmailId, renderedHtml);
				return true;
			}, "An OTP is sent to your registered email id for verification.");
		}
		public async Task<ServiceResponse<string>> ValidateAccessCode(string accessCode)
		{
			return await ServiceResponseExceptionHandler.Handle<string>(async () =>
			{
				var isValid = await _users.ValidateAccessCodeAsync(accessCode);
				if (isValid == false)
					throw new ServiceResponseException(System.Net.HttpStatusCode.OK, "Invalid access code.");
				return "Valid access code.";
			});
		}
		public async Task<ServiceResponse<string>> AddAccessCodeEmailAsync(AccessCodeEmailDto userAccessCodeDto)
		{
			return await ServiceResponseExceptionHandler.Handle<string>(async () =>
			{
				var userAccessCodeData = new UserAccessCode
				{
					Email = userAccessCodeDto.Email,
					OptionId = userAccessCodeDto.OptionId,
					ResendOtp = userAccessCodeDto.ResendOtp,
				};
				var (accessEmailData, msg) = await _users.AccessCodeEmail(userAccessCodeData);
				if (accessEmailData == null)
				{
					switch (msg)
					{
						case "UserExist":
							throw new ServiceResponseException(HttpStatusCode.BadRequest, "Email-id is already registered.");

						case "AccessCodeAssign":
							throw new ServiceResponseException(HttpStatusCode.BadRequest, "Please check your email for the access code and complete the registration process by entering the code provided.");
					}
				}
				if (accessEmailData != null && accessEmailData.OptionId == userAccessCodeDto.OptionId && !accessEmailData.IsOtpVerified)
				{
					return "Please verify OTP.";
				}
				var userAccessCode = await _users.AccessCodeCheck(userAccessCodeDto.Email, userAccessCodeDto.OptionId);

				if (userAccessCode != null && userAccessCode.IsOtpVerified)
				{
					if (userAccessCode.IsRejected == true)
					{
						throw new ServiceResponseException(HttpStatusCode.BadRequest, "Request for access code has been rejected.");
					}

					throw new ServiceResponseException(HttpStatusCode.BadRequest, "Request already send for access code.");
				}
				var htmlTemplate = LoadOtpHtmlTemplate("en-GB");
				var template = Template.Parse(htmlTemplate);
				var renderedHtml = template.Render(new { otp = accessEmailData.Otp, year = DateTime.Now.Year });
				_email.SendOtpMail(userAccessCodeDto.Email, renderedHtml);
				return "OTP sent to your email.";
			});
		}
		public async Task<ServiceResponse<string>> VerifyAccessCodeOtpAsync(VerifyAccessCodeOtp userAccessCodeDto)
		{
			return await ServiceResponseExceptionHandler.Handle<string>(async () =>
			{
				var (accessVerifyOtpData, msg) = await _users.VerifyAccessCodeOtp(userAccessCodeDto.Email, userAccessCodeDto.Otp, _languageId);
				if (accessVerifyOtpData == null)
				{
					throw new ServiceResponseException(HttpStatusCode.BadRequest, "Invalid OTP.");
				}

				if (!string.IsNullOrWhiteSpace(msg) && msg == "NoCode")
				{
					throw new ServiceResponseException(HttpStatusCode.BadRequest, "No code available.");
				}
				var htmlTemplate = LoadAccessCodeHtmlTemplate("en-GB");
				var template = Template.Parse(htmlTemplate);
				var renderedHtml = template.Render(new { accesscode = accessVerifyOtpData.AccessCode, year = DateTime.Now.Year });
				_email.SendAccessCodeMail(userAccessCodeDto.Email, renderedHtml);
				return "Access code sent to your email-id.";
			});
		}
		public async Task<ServiceResponse<string>> RequestAccessCodeAsync(UserAccessCodeRequestDto userAccessCodeRequestDto)
		{
			return await ServiceResponseExceptionHandler.Handle<string>(async () =>
			{
				var data = new UserAccessCode
				{
					Email = userAccessCodeRequestDto.Email,
					OptionId = userAccessCodeRequestDto.OptionId,
					OptionText = userAccessCodeRequestDto.OptionText,
					Message = userAccessCodeRequestDto.Message,
					ResendOtp = userAccessCodeRequestDto.ResendOtp,
					LanguageId = _languageId != 0 ? _languageId : 1,
				};
				var (accessEmailData, msg) = await _users.AccessCodeEmailOpt2(data);
				switch (msg)
				{
					case "UserExist":
						throw new ServiceResponseException(HttpStatusCode.BadRequest, "Email-id is already registered.");

					case "AccessCodeAssignORApproval":
						throw new ServiceResponseException(HttpStatusCode.BadRequest, "Access code has been already assigned or a request has been sent for approval.");

					case "requestedCode":
						throw new ServiceResponseException(HttpStatusCode.BadRequest, "Request already send for access code.");

					case "requestRejected":
						throw new ServiceResponseException(HttpStatusCode.BadRequest, "Request for access code has been rejected.");

					case "alreadyAssigned":
						throw new ServiceResponseException(HttpStatusCode.BadRequest, "Access code already assigned.");

					case "verifyOtp":
						return "Please verify OTP.";
				}
				var htmlTemplate = LoadOtpHtmlTemplate("en-GB");
				var template = Template.Parse(htmlTemplate);
				var renderedHtml = template.Render(new { otp = accessEmailData.Otp, year = DateTime.Now.Year });
				_email.SendOtpMail(userAccessCodeRequestDto.Email, renderedHtml);
				return "OTP sent to your email.";
			});
		}
		public async Task<ServiceResponse<string>> VerifyRequestAccessCodeOtpAsync(VerifyAccessCodeOtp userAccessCodeDto)
		{
			return await ServiceResponseExceptionHandler.Handle<string>(async () =>
			{
				var (accessVerifyOtpData, msg, optionText) = await _users.VerifyRequestAccessCodeOtp(userAccessCodeDto.Email, userAccessCodeDto.Otp);
				switch (msg)
				{
					case "InvalidOtp":
						throw new ServiceResponseException(HttpStatusCode.BadRequest, "Invalid OTP.");

					case "noUpdate":
						throw new ServiceResponseException(HttpStatusCode.BadRequest, "Not updated successfully.");
				}
				var requestAccessCodeEmail = _config["RequestAccessCodeMailId"];
				var expiryDays = _config["ExpiryDays"];
				var apiUrl = _config["ApiUrl"];

				string generateLsUrl = $"{apiUrl}?encdata={Uri.EscapeDataString(_encryptionService.Encrypt($"Type=LS&Email={userAccessCodeDto.Email}"))}";
				string generatePLUrl = $"{apiUrl}?encdata={Uri.EscapeDataString(_encryptionService.Encrypt($"Type=PLAY&Email={userAccessCodeDto.Email}"))}";
				string rejectUrl = $"{apiUrl}?encdata={Uri.EscapeDataString(_encryptionService.Encrypt($"Type=RJ&Email={userAccessCodeDto.Email}"))}";

				var htmlTemplate = LoadRequestAccessCodeHtmlTemplate("en-GB");
				var template = Template.Parse(htmlTemplate);
				var renderedHtml = template.Render(new { useremail = accessVerifyOtpData.Email, userselection = accessVerifyOtpData.OptionText, message = accessVerifyOtpData.Message, generateplurl = generatePLUrl, generatelsurl = generateLsUrl, rejecturl = rejectUrl, expiredays = expiryDays });
				_email.SendRequestAccessCodeMail(requestAccessCodeEmail, renderedHtml);
				return "Your request has been processed. The access code will be sent to your email.Thank you!";
			});
		}
		public async Task<ServiceResponse<string>> GenerateAccessCodeFromEmailAsync(AccessCodeRequestDto model)
		{
			return await ServiceResponseExceptionHandler.Handle<string>(async () =>
			{
				var accessRequestData = new AccessCodeRequest
				{
					Type = model.Type,
					Email = model.Email,
					ExpireDays = Convert.ToInt32(_config["ExpiryDays"])
				};
				var (accessVerifyOtpData, msg) = await _users.AssignAccessCodeFromEmail(accessRequestData);

				if (accessVerifyOtpData == null)
				{
					switch (msg)
					{
						case "expired":
							throw new ServiceResponseException(HttpStatusCode.BadRequest, "Sorry, your link has expired. Please contact the administrator.");

						case "alreadyAssigned":
							throw new ServiceResponseException(HttpStatusCode.BadRequest, "Access code already assigned.");

						case "NoCode":
							throw new ServiceResponseException(HttpStatusCode.BadRequest, "No code available.");

						case "NotFound":
							throw new ServiceResponseException(HttpStatusCode.BadRequest, "No user found.");

						case "Rejected":
							var rejectHtmlTemplate = LoadRejectAccessCodeHtmlTemplate("en-GB");
							var rejectTemplate = Template.Parse(rejectHtmlTemplate);
							var rejectRenderedHtml = rejectTemplate.Render(new { year = DateTime.Now.Year });
							_email.SendRejectAccessCodeMail(model.Email, rejectRenderedHtml);
							return "Access request rejected successfully.";

						case "alreadyRejected":
							throw new ServiceResponseException(HttpStatusCode.BadRequest, "Access code request already rejected.");
					}
				}

				var htmlTemplate = LoadAccessCodeHtmlTemplate("en-GB");
				var template = Template.Parse(htmlTemplate);
				var renderedHtml = template.Render(new { accesscode = accessVerifyOtpData?.AccessCode, year = DateTime.Now.Year });
				_email.SendAccessCodeMail(model.Email, renderedHtml);

				return "Access code is assigned successfully to the user.";
			});
		}
		public async Task<ServiceResponse<bool>> UpdateUserPasswordAsync(ChangePasswordDto changePasswordDto)
		{
			return await ServiceResponseExceptionHandler.Handle<bool>(async () =>
			{
				var user = await _users.GetByIdAsync(_userId);
				if (user == null)
					throw new ServiceResponseException(System.Net.HttpStatusCode.Conflict, "User Not Found");
				if (!BCrypt.Net.BCrypt.Verify(changePasswordDto.OldPassword, user.Password))
					throw new ServiceResponseException(System.Net.HttpStatusCode.Conflict, "Please enter the correct current password.");
				if (BCrypt.Net.BCrypt.Verify(changePasswordDto.NewPassword, user.Password))
					throw new ServiceResponseException(System.Net.HttpStatusCode.Conflict, "New password cannot be the same as the old password.");
				var userData = new ChangePassword
				{
					UserId = _userId,
					Password = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword),
					UpdatedBy = _userId,
				};
				var data = await _users.UpdateUserPasswordAsync(userData);
				return data;
			}, "User password updated successfully.");
		}
		public async Task<ServiceResponse<User>> ValidateTokenAsync(TokenRequestDto tokenRequestDto)
		{
			return await ServiceResponseExceptionHandler.Handle<User>(async () =>
			{
				var tokenDetail = await _users.GetRefreshToken(tokenRequestDto.RefreshToken);
				if (tokenDetail == null)
					throw new ServiceResponseException(System.Net.HttpStatusCode.Conflict, "Session token has expired or has already been used. Please log in again.");
				var user = await _users.GetByIdAsync(tokenDetail.UserId);
				if(user == null)
					throw new ServiceResponseException(System.Net.HttpStatusCode.Conflict, "User is disabled or inactive.");
				await _users.DeativeRefreshToken(tokenRequestDto.RefreshToken);
				var token = await GenerateJwtToken(user);
				var ipAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
				var refreshToken = Guid.NewGuid().ToString();
				DateTime? expireAt = DateTime.UtcNow.AddMinutes(5);
				var RefreshTokenExpiresDays = Convert.ToDouble(_config["Jwt:RefreshTokenValidityInDays"]);
				DateTime RefreshTokenExpiresAt = expireAt ?? DateTime.UtcNow.AddDays(RefreshTokenExpiresDays);
				DateTime RevokedAt = DateTime.UtcNow;
				var refreshTokenData = new RefreshTokenMaster
				{
					UserId = _userId,
					Token = refreshToken,
					CreatedByIp = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString(),
					ExpiresAt = RefreshTokenExpiresAt,
					RevokedAt = RevokedAt,
				};
				await _users.GenerateRefreshToken(refreshTokenData);
				user.Token = token;
				user.RefreshToken = refreshToken;
				user.Password = "";
				user.Otp = "";
				user.OtpVerifiedOn = null;
				return user;
			}, "Login Successfully.");
		}

		private UserDto MapToDto(User user)
		{
			return new UserDto
			{
				UserId = user.UserId,
				UserName = user.UserName,
				Gender = user.Gender,
				DateOfBirth = user.DateOfBirth,
				PhoneNumber = user.PhoneNumber,
				MobileNumber = user.MobileNumber,
				EmailId = user.EmailId,
				Address = user.Address,
				PostCode = user.PostCode,
				Country = user.Country,
				IsActive = user.IsActive,
				CreatedOn = user.CreatedOn,
				UpdatedOn = user.UpdatedOn,
				QuestionnaireId = user.QuestionnaireId,
			};
		}

		private string LoadHtmlTemplate(string lCode)
		{
			var htmlFilePath = Path.Combine(AppContext.BaseDirectory, "Html", $"ForgotpasswordMail_{lCode}.html");
			if (!System.IO.File.Exists(htmlFilePath))
			{
				htmlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Html", $"ForgotpasswordMail_en-GB.html");
			}
			return System.IO.File.ReadAllText(htmlFilePath);
		}

		private string LoadOtpHtmlTemplate(string lCode)
		{
			var htmlFilePath = Path.Combine(AppContext.BaseDirectory, "Html", $"OtpMail_{lCode}.html");
			if (!System.IO.File.Exists(htmlFilePath))
			{
				htmlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Html", $"OtpMail_en-GB.html");
			}
			return System.IO.File.ReadAllText(htmlFilePath);
		}
		private string LoadAccessCodeHtmlTemplate(string lCode)
		{
			var htmlFilePath = Path.Combine(AppContext.BaseDirectory, "Html", $"AccessCodeMail_{lCode}.html");
			if (!System.IO.File.Exists(htmlFilePath))
			{
				htmlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Html", $"AccessCodeMail_en-GB.html");
			}
			return System.IO.File.ReadAllText(htmlFilePath);
		}
		private string LoadRequestAccessCodeHtmlTemplate(string lCode)
		{
			var htmlFilePath = Path.Combine(AppContext.BaseDirectory, "Html", $"RequestAccessCode_{lCode}.html");
			if (!System.IO.File.Exists(htmlFilePath))
			{
				htmlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Html", $"RequestAccessCode_en-GB.html");
			}
			return System.IO.File.ReadAllText(htmlFilePath);
		}
		private string LoadRejectAccessCodeHtmlTemplate(string lCode)
		{
			var htmlFilePath = Path.Combine(AppContext.BaseDirectory, "Html", $"AccessRequestRejected_{lCode}.html");
			if (!System.IO.File.Exists(htmlFilePath))
			{
				htmlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Html", $"AccessRequestRejected_en-GB.html");
			}
			return System.IO.File.ReadAllText(htmlFilePath);
		}
	}
}

