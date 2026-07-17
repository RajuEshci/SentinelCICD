using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentinelApp.Domain.Interfaces
{
	public interface ICheckDuplicateRepository
	{
		Task<string?> CheckDuplicateAsync(string tableName, Dictionary<string, object?> fieldValues, string mode = "single", string? excludeIdColumn = null, object? excludeIdValue = null);
	}
}
