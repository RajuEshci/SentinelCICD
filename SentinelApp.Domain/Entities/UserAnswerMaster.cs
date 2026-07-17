using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentinelApp.Domain.Entities
{
	public class UserAnswerMaster : CommonFields
	{
		public int UserAnswerId { get; set; }
		public int UserId { get; set; }
		public int QuestionId { get; set; }
		public int? OptionId { get; set; }
		public string? Answer { get; set; }
		public DateTime? AnswerDate { get; set; }
	}
	public class UserAnswerHistoryMaster : CommonFields
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
	public class UserAnswerHistoryFilter
	{
		public int UserId { get; set; }
		public int QuestionId { get; set; }
		public string? Status { get; set; }
		public string? SortColumn { get; set; }
		public string? SortDirection { get; set; }
		public int PageNo { get; set; }
		public int PerPage { get; set; }
		public int Offset { get; set; }
		public bool IsActive { get; set; }
	}
	public class QuestionnaireAnswer
	{
        public int LanguageId { get; set; }
        public string? ParentQuestionCode { get; set; }
        public List<int>? QuestionIds { get; set; }
        public int QuestionId { get; set; }
		public string? QuestionCode { get; set; }
		public string? QuestionType { get; set; }
		public bool IsMain { get; set; }
		public int DisplayOrder { get; set; }
		public string? QuestionText { get; set; }
		public string? ActionName { get; set; }
		public int UserId { get; set; }
		public int OptionId { get; set; }
		public string? OptionValue { get; set; }
		public string? Optiontext { get; set; }
		public string? Answer { get; set; }
		public string? AnswerDate { get; set; }
		public string? SelectedAnswer { get; set; }
	}
	public class UserActionMaster:CommonFields
	{
		public int ActionId { get; set; }
		public string? ActionName { get; set; }
		public DateTime? DueDate { get; set; }
		public string? Status { get; set; }
		public string? ColorCode { get; set; }
		public int SortOrder { get; set; }
		public int QuestionId { get; set; }
		public int UserId { get; set; }
	}
	public class UserPDFDataMaster
	{
        public int UserAnswerId { get; set; }
        public int QuestionId { get; set; }
        public int DisplayOrder { get; set; }
        public string? QuestionText { get; set; }
        public DateTime? AnswerDate { get; set; }
        public string? Answer { get; set; }
        public bool IsMain { get; set; }
    }
}
