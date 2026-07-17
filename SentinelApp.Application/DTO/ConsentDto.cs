using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentinelApp.Application.DTO
{
	public class ConsentDto
	{
		public int Id { get; set; }
		public string? Description { get; set; }
		public int QuestionnarieId { get; set; }
		public bool FlagDeleted { get; set; }
		public DateTime CreatedOn { get; set; }
		public DateTime UpdatedOn { get; set; }
	}
	public class UserConsentAnswerDto
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
		public List<UserConsentAnswerDto>? Consents { get; set; }
	}

	public class ConsentResponseDto
	{
		public int ConsentId { get; set; }
		public string? Description { get; set; }
		public string? Answer { get; set; }
		public int QuestionnarieId { get; set; }
		public string? LinkText { get; set; }
		public bool Required { get; set; }
	}
	public class ConsentUserRequestDto
	{
        public int ConsentId { get; set; }
        public string? Answer { get; set; }
    }
	public class ConsentRequestDto
	{
        public int QuestionnarieId { get; set; }
    }
}
