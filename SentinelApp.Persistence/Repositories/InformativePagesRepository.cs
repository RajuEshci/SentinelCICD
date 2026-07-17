using Dapper;
using SentinelApp.Domain.Entities;
using SentinelApp.Domain.Interfaces;
using SentinelApp.Persistence.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace SentinelApp.Persistence.Repositories
{
	public class InformativePagesRepository : IInformativePages
	{
		private readonly IDbContext _dbContext;

		public InformativePagesRepository(IDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<int> AddAsync(InformativePagesMaster entity)
		{
			using var _connection = _dbContext.CreateConnection();
			var sql = @"Insert into InformativePages(Name,Description,Image,QuestionnaireId,AndriodUrl,IosUrl,SortOrder,ParentPageId,IsApplication,CreatedBy,CreatedOn,PageCode,LanguageId,IsMenu,IsActive)
						OUTPUT INSERTED.Id
						VALUES(@Name,@Description,@Image,@QuestionnaireId,@AndriodUrl,@IosUrl,@SortOrder,@ParentPageId,@IsApplication,@CreatedBy,GETDATE(),@PageCode,@LanguageId,@IsMenu,@IsActive)";
			var id = await _connection.ExecuteScalarAsync<int>(sql, entity);
			return id;
		}

		public async Task<bool> DeleteAsync(int id, int userId)
		{
			using var _connection = _dbContext.CreateConnection();
			var sql = @"Update InformativePages set FlagDeleted=1, DeletedOn=GETDATE() where Id=@Id";
			var data = await _connection.QueryFirstOrDefaultAsync<bool>(sql, new { Id = id });
			return data;
		}

		public async Task<IEnumerable<InformativePagesMaster>> GetAllAsync(InformativePagesFilter filter)
		{
			using var _connection = _dbContext.CreateConnection();
			var sql = @"select * from InformativePages where FlagDeleted=0";
			if (!string.IsNullOrEmpty(filter.SearchText))
			{
				sql += @" AND (Name LIKE '%' + @SearchText + '%' OR
                    Description LIKE '%' + @SearchText + '%')";
			}
			if (!string.IsNullOrEmpty(filter.Status))
			{
				sql += " AND IsActive = @IsActive";
				filter.IsActive = filter.Status.ToLower() == "active" ? true : false;
			}
			var sortCol = !string.IsNullOrEmpty(filter.SortColumn) ? filter.SortColumn : "CreatedOn";
			var sortDirection = !string.IsNullOrEmpty(filter.SortDirection) ? filter.SortDirection : "DESC";
			sql += $" ORDER BY {sortCol} {sortDirection}";

			sql += " OFFSET @Offset ROWS FETCH NEXT @PerPage ROWS ONLY";
			filter.Offset = (filter.PageNo - 1) * filter.PerPage;
			return (await _connection.QueryAsync<InformativePagesMaster>(sql, filter)).ToList();
		}

		public async Task<InformativePagesMaster?> GetByIdAsync(int id)
		{
			using var _connection = _dbContext.CreateConnection();
			return await _connection.QueryFirstOrDefaultAsync<InformativePagesMaster>(@"SELECT * FROM InformativePages WHERE Id = @Id and FlagDeleted=0 AND IsActive=1",
			new { Id = id });
		}

		public async Task<bool> UpdateAsync(InformativePagesMaster entity)
		{
			using var _connection = _dbContext.CreateConnection();
			var sql = @"UPDATE InformativePages SET 
					Name=@Name,
					Description=@Description
					Image=@Image,
					QuestionnaireId=@QuestionnaireId,
					AndriodUrl=@AndriodUrl,
					IosUrl=@IosUrl,
					SortOrder=@SortOrder,
					ParentPageId=@ParentPageId,
					IsApplication=@IsApplication,
					UpdatedBy=@UpdatedBy,
					UpdatedOn=GETDATE(),
					PageCode=@PageCode,
					LanguageId=@LanguageId,
					IsMenu=@IsMenu,
					IsActive=@IsActive
                   WHERE Id = @Id";

			var rowsAffected = await _connection.ExecuteAsync(sql, entity);
			return rowsAffected > 0;
		}
		public async Task<InformativePagesMaster?> GetByPageCodeIdAsync(string pgCode, int? questionnaireId)
		{
			using var _connection = _dbContext.CreateConnection();

			const string userSql = @"SELECT * FROM InformativePages WHERE QuestionnaireId = @QuestionnaireId AND PageCode=@PageCode AND FlagDeleted = 0 AND IsActive=1";
			var data = await _connection.QueryFirstOrDefaultAsync<InformativePagesMaster>(userSql, new { QuestionnaireId = questionnaireId, PageCode = pgCode });
			if (data == null) return null;

			return data;
		}
		public async Task<List<InformativePagesMaster>?> GetInformativePagesListAsync(int questionnaireId)
		{
			using var _connection = _dbContext.CreateConnection();

			const string sql = @"SELECT * FROM InformativePages WHERE IsActive=1 AND FlagDeleted = 0 AND QuestionnaireId = @QuestionnaireId";
			var data = (await _connection.QueryAsync<InformativePagesMaster>(sql, new { QuestionnaireId = questionnaireId })).ToList();
			if (data == null) return null;

			return data;
		}
		public async Task<CardCount> GetAllCardCounts(InformativePagesFilter filter)
		{
			using var _connection = _dbContext.CreateConnection();
			var countSql = @"SELECT 
				COUNT(*) AS Total,
				SUM(CASE WHEN IsActive = 1 THEN 1 ELSE 0 END) AS ActiveCount,
				SUM(CASE WHEN IsActive = 0 THEN 1 ELSE 0 END) AS InactiveCount
			FROM InformativePages
			WHERE FlagDeleted=0";

			if (!string.IsNullOrEmpty(filter.SearchText))
			{
				countSql += @" AND (Name LIKE '%' + @SearchText + '%' OR
                    Description LIKE '%' + @SearchText + '%')";
			}
			if (!string.IsNullOrEmpty(filter.Status))
			{
				countSql += " AND IsActive = @IsActive";
				filter.IsActive = filter.Status.ToLower() == "active" ? true : false;
			}

			return await _connection.QueryFirstAsync<CardCount>(countSql, filter);
		}
		public async Task<List<InformativePagesMaster>?> GetMenuListAsync(int questionnaireId, string gender)
		{
			using var _connection = _dbContext.CreateConnection();

			const string userSql = @"SELECT * FROM InformativePages WHERE QuestionnaireId = @QuestionnaireId AND (Gender = @Gender OR Gender IS NULL) AND FlagDeleted = 0 AND IsActive=1 and IsMenu=1";
			var data = (await _connection.QueryAsync<InformativePagesMaster>(userSql, new { QuestionnaireId = questionnaireId, Gender = gender })).ToList();
			if (data == null) return null;

			return data;
		}
	}
}
