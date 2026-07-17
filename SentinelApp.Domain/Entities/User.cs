using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SentinelApp.Domain.Entities;

namespace SentinelApp.Domain.Entities
{
	public class User:CommonFields
	{
		public int UserId { get; set; }
		public string UserName { get; set; }
		public string Gender { get; set; }
		public DateTime DateOfBirth { get; set; }
		public string PhoneNumber { get; set; }
		public string MobileNumber { get; set; }
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
		public DateTime? OtpVerifiedOn { get; set; }
		public bool IsOtpVerify { get; set; }
		public int OtpAttempts { get; set; }
		public string AccessCode { get; set; }
		public bool IsAdmin { get; set; }
		public bool IsConsentUpdated { get; set; }
		public string? PreferredLanguageCode { get; set; }
		public int PreferredLanguageId { get; set; }
		public bool IsExported { get; set; }
		public string Version { get; set; }
		public string Model { get; set; }
        public string Token { get; set; }
		public int QuestionnaireId { get; set; }
		public string RefreshToken { get; set; }
	}
    public class UserRoleMaster : CommonFields
    {
        public int UserRoleId { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public string RoleNames { get; set; } = "";
        public string RoleName { get; set; } = "";
    }

    public class UserFilter
	{
		public string? RoleName { get; set; }
		public string? SearchText { get; set; }
		public string? Status { get; set; }
		public string? SortColumn { get; set; }
		public string? SortDirection { get; set; }
		public int PageNo { get; set; }
		public int PerPage { get; set; }
		public int Offset { get; set; }
		public bool IsActive { get; set; }
	}
	public class ChangePassword
	{
        public int UserId { get; set; }
        public string? Password { get; set; }
        public int UpdatedBy { get; set; }
    }
	public class RefreshTokenMaster
	{
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime RevokedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedByIp { get; set; }
        public string RevokedByIp { get; set; }
        public string ReplacedByToken { get; set; }
        public bool IsRevoked { get; set; }
        public bool IsUsed { get; set; }
    }
}
