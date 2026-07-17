using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentinelApp.Domain.Entities
{
	public class InformativePagesMaster : CommonFields
	{
		public int Id { get; set; }
		public string? Name { get; set; }
		public string? Description { get; set; }
		public string? Image { get; set; }
		public int QuestionnaireId { get; set; }
		public string? AndriodUrl { get; set; }
		public string? IosUrl { get; set; }
		public int SortOrder { get; set; }
		public int ParentPageId { get; set; }
		public bool IsApplication { get; set; }
		public bool FlagDeleted { get; set; }
		public DateTime CreatedOn { get; set; }
		public DateTime UpdatedOn { get; set; }
		public string? PageCode { get; set; }
		public int LanguageId { get; set; }
		public int IsMenu { get; set; }
	}
	public class InformativePagesFilter
	{
		public string? SearchText { get; set; }
		public string? Status { get; set; }
		public string? SortColumn { get; set; }
		public string? SortDirection { get; set; }
		public int PageNo { get; set; }
		public int PerPage { get; set; }
		public int Offset { get; set; }
		public bool IsActive { get; set; }
	}
}
