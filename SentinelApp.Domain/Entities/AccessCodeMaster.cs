using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentinelApp.Domain.Entities
{
	public class AccessCodeMaster
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
	public class UserAccessCode
	{
		public int Id { get; set; }
		public string Email { get; set; }
		public string Otp { get; set; }
		public bool IsOtpVerified { get; set; }
		public string OtpSentOn { get; set; }
		public DateTime? OtpVerifiedOn { get; set; }
		public DateTime CreatedOn { get; set; }
		public int OptionId { get; set; }
		public DateTime UpdatedOn { get; set; }
		public string Message { get; set; }
		public bool IsRejected { get; set; }
		public DateTime RejectedOn { get; set; }
		public int LanguageId { get; set; }
		public string OptionText { get; set; }
        public bool ResendOtp { get; set; }
        public string AccessCode { get; set; }
    }
	public class AccessCodeRequest
	{
		public string Type { get; set; }
		public string Email { get; set; }
		public int ExpireDays { get; set; }
	}
	public class LanguageMaster
	{
		public int LanguageId { get; set; }
		public string? LanguageCode { get; set; }
		public string? LanguageName { get; set; }
	}
}
