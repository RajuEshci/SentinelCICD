using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentinelApp.Domain.Entities
{
	public class CardCount
	{
		public int Total { get; set; }
		public int ActiveCount { get; set; }
		public int InactiveCount { get; set; }
	}
}
