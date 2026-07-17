using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentinelApp.Application.DTO
{
	public class PaginatedResponseDto<T>
	{
		public int Page { get; set; }
		public int PerPage { get; set; }
		public int Total { get; set; }
		public int TotalPages { get; set; }
		public List<T> Data { get; set; } = new List<T>();
		public CardCountDto CardCount { get; set; } = new CardCountDto();
	}

	public class CardCountDto
	{
		public int Total { get; set; }
		public int ActiveCount { get; set; }
		public int InactiveCount { get; set; }
		public DateTime? LastUpdated { get; set; }
	}
}
