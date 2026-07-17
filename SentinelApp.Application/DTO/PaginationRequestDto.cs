using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentinelApp.Application.DTO
{
	public class PaginationRequestDto
	{
		public int Page { get; set; } = 1;
		public int PerPage { get; set; } = 10;
		public string? SortColumn { get; set; }
		public string? SortDirection { get; set; }
	}
}
