using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentinelApp.Domain.Entities
{
	public class Consent:CommonFields
	{
		public int Id { get; set; }
		public string? Description { get; set; }
		public bool FlagDeleted { get; set; }
		public DateTime CreatedOn { get; set; }
		public DateTime UpdatedOn { get; set; }
	}
	public class UserConsentAnswer
	{
		public int Id { get; set; }
		public int ConsentId { get; set; }
		public int UserId { get; set; }
		public string? Answer { get; set; }
		public bool FlagDeleted { get; set; }
		public DateTime CreatedOn { get; set; }
		public int CreatedBy { get; set; }
		public DateTime UpdatedOn { get; set; }
		public int UpdatedBy { get; set; }
		public List<UserConsentAnswer>? Consents { get; set; }
	}

	public class ConsentRequest
	{
		public int ConsentId { get; set; }
		public string? Description { get; set; }
		public string? Answer { get; set; }
		public int QuestionnarieId { get; set; }
		public string? LinkText { get; set; }
		public bool Required { get; set; }
	}
}
