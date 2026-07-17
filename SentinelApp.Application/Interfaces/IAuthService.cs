using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SentinelApp.Application.Core;
using SentinelApp.Application.DTO;
using SentinelApp.Application.DTO.Response;
using SentinelApp.Domain.Entities;

namespace SentinelApp.Application.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResponse<bool?>> DBHealthy();
        Task<ServiceResponse<bool?>> CreateUser(RegisterDto dto);
        Task<ServiceResponse<bool?>> UpdateUser(UpdateUserDto dto);
        Task<ServiceResponse<UserDto>> GetByUserId(int Id);
        Task<ServiceResponse<bool>> DeleteById();
        Task<ServiceResponse<User?>> LoginAsync(LoginDto dto);
        Task<ServiceResponse<bool>> ForgotPasswordAsync(string email);
        Task<ServiceResponse<bool>> VerifyOtp(VerifyOtpDto verifyOtpDto);
        Task<ServiceResponse<bool>> ResendOtp(VerifyOtpDto verifyOtpDto);
        Task<ServiceResponse<string>> ValidateAccessCode(string accessCode);
        Task<ServiceResponse<string>> AddAccessCodeEmailAsync(AccessCodeEmailDto userAccessCodeDto);
        Task<ServiceResponse<string>> VerifyAccessCodeOtpAsync(VerifyAccessCodeOtp userAccessCodeDto);
        Task<ServiceResponse<string>> RequestAccessCodeAsync(UserAccessCodeRequestDto userAccessCodeRequestDto);
        Task<ServiceResponse<string>> VerifyRequestAccessCodeOtpAsync(VerifyAccessCodeOtp userAccessCodeDto);
		Task<ServiceResponse<string>> GenerateAccessCodeFromEmailAsync(AccessCodeRequestDto model);
        Task<ServiceResponse<bool>> UpdateUserPasswordAsync(ChangePasswordDto changePasswordDto);
        Task<ServiceResponse<bool>> DisableUser();
        Task<ServiceResponse<User>> ValidateTokenAsync(TokenRequestDto tokenRequestDto);
	}
}
