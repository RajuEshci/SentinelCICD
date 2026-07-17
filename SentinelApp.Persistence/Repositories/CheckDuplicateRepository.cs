using Dapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SentinelApp.Domain.Interfaces;
using SentinelApp.Persistence.Data;

namespace SentinelApp.Persistence.Repositories
{
	public class CheckDuplicateRepository : ICheckDuplicateRepository
	{
		private readonly IDbContext _context;
		private readonly ILogger<CheckDuplicateRepository> _logger;

		public CheckDuplicateRepository(IDbContext context, ILogger<CheckDuplicateRepository> logger)
		{
			_context = context;
			_logger = logger;
		}
		public async Task<string?> CheckDuplicateAsync(string tableName, Dictionary<string, object?> fieldValues, string mode = "single", string? excludeIdColumn = null, object? excludeIdValue = null)
		{
			using var connection = _context.CreateConnection();

			string excludeCondition = "";
			if (!string.IsNullOrEmpty(excludeIdColumn) && excludeIdValue != null)
				excludeCondition = $" AND {excludeIdColumn} <> @excludeIdValue";

			if (mode.Equals("single", StringComparison.OrdinalIgnoreCase))
			{
				foreach (var kvp in fieldValues)
				{
					var columnName = kvp.Key;
					var columnValue = kvp.Value;
					if (columnValue == null || (columnValue is string str && string.IsNullOrEmpty(str)))
					{
						continue;
					}
					var sql = $"SELECT CASE WHEN EXISTS (SELECT 1 FROM {tableName} WHERE {columnName} = @value and FlagDeleted=0 {excludeCondition}) THEN 1 ELSE 0 END";

					var exists = await connection.ExecuteScalarAsync<bool>(sql, new { value = columnValue, excludeIdValue });

					if (exists)
					{
						var prettyName = FormatColumnName(columnName);
						return $"{prettyName} already exists.";
					}
				}
			}
			// MULTIPLE MODE: check each column, collect duplicates
			else if (mode.Equals("multiple", StringComparison.OrdinalIgnoreCase))
			{
				var duplicateColumns = new List<string>();
				var whereClauses = new List<string>();
				var parameters = new DynamicParameters();
				foreach (var kvp in fieldValues)
				{
					var columnName = kvp.Key;
					var columnValue = kvp.Value;

					foreach (var kv in fieldValues)
					{
						if (kv.Value == null || (kv.Value is string str && string.IsNullOrWhiteSpace(str)))
						{
							continue;
						}
						whereClauses.Add($"{kv.Key} = @{kv.Key}");
						parameters.Add(kv.Key, kv.Value);
					}
					if (!string.IsNullOrEmpty(excludeIdColumn) && excludeIdValue != null)
					{
						whereClauses.Add($"{excludeIdColumn} <> @excludeIdValue");
						parameters.Add("excludeIdValue", excludeIdValue);
					}
					string whereCondition = string.Join(" AND ", whereClauses);

					var sql = $"SELECT CASE WHEN EXISTS (SELECT 1 FROM {tableName} WHERE {whereCondition} and FlagDeleted=0) THEN 1 ELSE 0 END";

					var exists = await connection.ExecuteScalarAsync<bool>(sql, parameters);

					if (exists)
					{
						var colNames = fieldValues.Keys.Select(k => FormatColumnName(k)).ToList();
						string message = string.Join(", ", colNames.Take(colNames.Count - 1)) +
							 (colNames.Count > 1 ? " and " + colNames.Last() : colNames.First());
						//duplicateColumns.Add(FormatColumnName(columnName));
						return $"{message} already exists.";
					}
				}

				//if (duplicateColumns.Any())
				//{
				//	// build a nice message: "Code and Name already exist."
				//	string message = string.Join(", ", duplicateColumns.Take(duplicateColumns.Count - 1))
				//						+ (duplicateColumns.Count > 1 ? " and " + duplicateColumns.Last() : duplicateColumns.First());

				//	return $"{message} already exist.";
				//}
			}

			return null;
		}

		// helper to format column names ("ActionNarrativeDescription" -> "Action narrative description")
		private string FormatColumnName(string columnName)
		{
			var custumNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
			{
				{ "BbergTicker","Ticker" },
				{ "EntityID","Entity" },
				{ "PortfolioID","Portfolio" },
				{ "SecurityID","Security" },
				//{ "IBAccountNo","IB account number" },
			};
			if (custumNames.TryGetValue(columnName, out var friendly))
				return friendly;
			var colNm = System.Text.RegularExpressions.Regex.Replace(columnName, "([a-z])([A-Z])", "$1 $2").ToLower();
			return char.ToUpper(colNm[0]) + colNm.Substring(1);
		}
	}
}
