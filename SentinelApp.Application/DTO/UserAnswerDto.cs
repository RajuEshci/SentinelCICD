using SentinelApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentinelApp.Application.DTO
{
	public class UserAnswerDto
	{
		public int UserAnswerId { get; set; }
		public int UserId { get; set; }
		public int QuestionId { get; set; }
		public int OptionId { get; set; }
		public string? Answer { get; set; }
		public DateTime? AnswerDate { get; set; }
		public int CreatedBy { get; set; }
		public int UpdatedBy { get; set; }
	}
	public class UpsertUserAnswerDto
	{
		public List<AddAnswerDto> Add { get; set; }
		public List<RemoveAnswerDto> Remove { get; set; }
	}
	public class AddAnswerDto
	{
		public int UserAnswerId { get; set; }
		public int QuestionId { get; set; }
		public int? OptionId { get; set; }
		public string? Answer { get; set; }
		public DateTime? AnswerDate { get; set; }
	}
	public class RemoveAnswerDto
	{
		public int UserAnswerId { get; set; }
	}
	public class UserAnswerReponseDto
	{
		public int UserAnswerId { get; set; }
		public int UserId { get; set; }
		public int QuestionId { get; set; }
		public int? OptionId { get; set; }
		public string? Answer { get; set; }
		public DateTime? AnswerDate { get; set; }
	}
	public class UserAnswerHistoryDto
	{
		public int UserAnswerHistoryId { get; set; }
		public int UserAnswerId { get; set; }
		public int UserId { get; set; }
		public int QuestionId { get; set; }
		public int? OptionId { get; set; }
		public string? Answer { get; set; }
		public DateTime? AnswerDate { get; set; }
		public string? ActionType { get; set; }
		public DateTime ActionDate { get; set; }
		public DateTime CreatedOn { get; set; }
		public DateTime DeletedOn { get; set; }
		public DateTime UpdatedOn { get; set; }
        public bool FlagDeleted { get; set; }
    }
	public class UserAnswerHistoryFilterDto : PaginationRequestDto
	{
		public int QuestionId { get; set; }
		public string? Status { get; set; }
	}
	public class UserActionResponseDto
	{
		public int ActionId { get; set; }
		public string? ActionName { get; set; }
		public DateTime? DueDate { get; set; }
		public string? Status { get; set; }
		public string? ColorCode { get; set; }
		//public int SortOrder { get; set; }
		public int QuestionId { get; set; }
		public int UserId { get; set; }
	}
	public class MedicalHistoryPDFDto
	{
		public List<MedicalHistoryQuestionDto> Questions { get; set; } = new();
	}
	public class MedicalHistoryQuestionDto
	{
		public string Question { get; set; }
		public List<string> Answers { get; set; } = new();
	}
}
