using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SentinelApp.Application.DTO;
using SentinelApp.Domain.Entities;

namespace SentinelApp.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> DBHealthy();
        Task<User?> GetByEmailAsync(string email,int Id);
        Task<User?> GetByIdAsync(int Id);
        Task<User?> GetByUsernameAsync(string userName,int Id);
        Task<int> CreateAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(int userId);
        Task<bool> DisableUserAsync(int userId);
		Task<IEnumerable<User>> GetAllUsersAsync(UserFilter userFilter);
        Task<User?> ValidateLogin(string emailId);
        Task UpdateOtpAsync(User user);
        Task VerifyOtpAsync(User user);
        Task<bool> ValidateAccessCodeAsync(string accessCode);
        Task<(UserAccessCode userAccessCode, string msg)> AccessCodeEmail(UserAccessCode userAccessCode);
        Task<UserAccessCode?> AccessCodeCheck(string email, int optionId);
        Task<(UserAccessCode userAccessCode, string error)> VerifyAccessCodeOtp(string email, string otp, int languageId);
        Task<(UserAccessCode userAccessCode, string msg)> AccessCodeEmailOpt2(UserAccessCode model);
        Task<(UserAccessCode userAccessCode, string error, string optionText)> VerifyRequestAccessCodeOtp(string email, string otp);
        Task<(AccessCodeMaster? accessCodeMaster, string error)> AssignAccessCodeFromEmail(AccessCodeRequest accessCodeRequest);
        Task<bool> UpdateUserPasswordAsync(ChangePassword changePassword);
        Task<AccessCodeMaster?> GetAccessCodeAsync(string accessCode);
        Task<LanguageMaster?> GetLanguageFromAccessCodeAsync(string accessCode);
        Task<bool> DeactivateUserAsync(int userId, string email);
        Task GenerateRefreshToken(RefreshTokenMaster refreshTokenMaster);
        Task<RefreshTokenMaster> GetRefreshToken(string refreshToken);
        Task DeativeRefreshToken(string refreshToken);
	}
}
