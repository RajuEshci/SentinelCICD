using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentinelApp.Application.DTO
{
    public class UserDto
    {
        public int UserId { get; set; }
		public string UserName { get; set; }
		public string Gender { get; set; }
		public DateTime DateOfBirth { get; set; }
		public string PhoneNumber { get; set; }
		public string MobileNumber { get; set; }

		[Required(ErrorMessage = "Email is required.")]
		public string EmailId { get; set; }
		public string Address { get; set; }
		public string PostCode { get; set; }
		public string Country { get; set; }
		public string Password { get; set; }
		public string Activationcode { get; set; }
		public bool Is2FactorAuthentication { get; set; }
		public int AuthenticationId { get; set; }
		public bool IsVerify { get; set; }
		public bool IsEmailVerify { get; set; }
		public bool IsMobileVerify { get; set; }
		public bool IsActive { get; set; }
		public bool IsSuspend { get; set; }
		public DateTime CreatedOn { get; set; }
		public DateTime UpdatedOn { get; set; }
		public bool IsTnCChecked { get; set; }
		public DateTime ActivationCodeGenOn { get; set; }
		public bool IsConsent { get; set; }
		public string ConsentLabelId { get; set; }
		public string SubscriptionId { get; set; }
		public DateTime SubscriptionUpdatedDate { get; set; }
		public string Otp { get; set; }
		public DateTime OtpGenOn { get; set; }
		public DateTime OtpVerifiedOn { get; set; }
		public bool IsOtpVerify { get; set; }
		public int OtpAttempts { get; set; }
		public string AccessCode { get; set; }
		public bool IsAdmin { get; set; }
		public bool Flagdeleted { get; set; }
		public bool IsConsentUpdated { get; set; }
		public string PreferredLanguageCode { get; set; }
		public int PreferredLanguageId { get; set; }
		public bool IsExported { get; set; }
		public string Version { get; set; }
		public string Model { get; set; }
		public int QuestionnaireId { get; set; }
	}  
	public class UserRoles
	{
        public int RoleId { get; set; }
		public string? RoleName { get; set; }
    }
	public class ResetUserPasswordDto
	{
		[Required(ErrorMessage = "Email is required.")]
		public string Email { get; set; }

		[Required(ErrorMessage = "OTP is required.")]
		public string OTP { get; set; }

		[Required(ErrorMessage = "New password is required.")]
		public string NewPassword { get; set; }
    }
	public class ForgotPasswordDto
	{
        public string EmailId { get; set; }
    }
	public class VerifyOtpDto
	{
		public string EmailId { get; set; }
		public string Otp { get; set; }
	}
	public class RegisterDto
	{
		public string? UserName { get; set; }
		public string? Gender { get; set; }
		public DateTime DateOfBirth { get; set; }
		public string? PhoneNumber { get; set; }
		public string? MobileNumber { get; set; }

		[Required(ErrorMessage = "Email is required.")]
		public string EmailId { get; set; }
		public string? Address { get; set; }
		public string? PostCode { get; set; }
		public string? Country { get; set; }
		public string? Password { get; set; }
		public bool IsTnCChecked { get; set; }
		public string? AccessCode { get; set; }
		public int QuestionnaireId { get; set; }
	}
	public class ChangePasswordDto
	{
        public string? OldPassword { get; set; }
        public string? NewPassword { get; set; }
    }
	public class DisableUserDto
	{
        public string EmailId { get; set; }
    }
	public class UpdateUserDto
	{
        public string? UserName { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public string? PostCode { get; set; }
        public string? Country { get; set; }
        public string? PhoneNumber { get; set; }
        public string? MobileNumber { get; set; }
        public string? EmailId { get; set; }
    }
	public class TokenRequestDto
	{
        public string? RefreshToken { get; set; }
    }
}
