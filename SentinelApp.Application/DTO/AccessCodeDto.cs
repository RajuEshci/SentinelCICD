using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentinelApp.Application.DTO
{
	public class AccessCodeDto
	{
		public int AccessCode_Id { get; set; }
		public string AccessCode { get; set; }
		public int AccessCodeStatus { get; set; }
		public bool Isdelete { get; set; }
		public DateTime CreatedOn { get; set; }
		public DateTime ModifiedOn { get; set; }
		public string AssignedTo { get; set; }
		public int AccessCodeTypeId { get; set; }
		public int LanguageId { get; set; }
	}
	public class ValidateAccessCodeDto
	{
		public string AccessCode { get; set; }
	}
	public class UserAccessCodeDto
	{
		public int Id { get; set; }
		public string Email { get; set; }
		public string Otp { get; set; }
		public string IsOtpVerified { get; set; }
		public string OtpSentOn { get; set; }
		public string OtpVerifiedOn { get; set; }
		public DateTime CreatedOn { get; set; }
		public int OptionId { get; set; }
		public DateTime UpdatedOn { get; set; }
		public string Message { get; set; }
		public bool IsRejected { get; set; }
		public DateTime RejectedOn { get; set; }
		public int LanguageId { get; set; }
		public string OptionText { get; set; }
		public bool ResendOtp { get; set; }
	}
	public class UserAccessCodeRequestDto
	{
		public string Email { get; set; }
		public int OptionId { get; set; }
		public string OptionText { get; set; }
		public string Message { get; set; }
		public bool ResendOtp { get; set; }
	}
	public class AccessCodeRequestDto
	{
		public string? Type { get; set; }
		public string? Email { get; set; }
	}
	public class VerifyAccessCodeOtp
	{
		public string Email { get; set; }
		public string Otp { get; set; }
	}
	public class AccessCodeEmailDto
	{
		public string Email { get; set; }
		public int OptionId { get; set; }
		public bool ResendOtp { get; set; }
	}
}
