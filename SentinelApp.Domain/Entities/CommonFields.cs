using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentinelApp.Domain.Entities
{
	public class CommonFields
	{
        public bool IsActive { get; set; }
		public int CreatedBy { get; set; }
		public int UpdatedBy { get; set; }
    }
}
