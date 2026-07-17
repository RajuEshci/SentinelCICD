using Dapper;
using SentinelApp.Domain.Entities;
using SentinelApp.Domain.Interfaces;
using SentinelApp.Persistence.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentinelApp.Persistence.Repositories
{
	public class ConsentRepository : IConsent
	{
		private readonly IDbContext _dbContext;
		public ConsentRepository(IDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<List<ConsentRequest>> GetConsentsAsync(int userId, string configConsent, int questionnarieId)
		{
			using var _connection = _dbContext.CreateConnection();
			var sql = @"SELECT 
					c.Id AS ConsentId,
					c.Description,c.LinkText,
					c.QuestionnaireId AS QuestionnaireId,
					ISNULL(ua.Answer, 'No') AS Answer
				FROM consents c
				LEFT JOIN userconsentanswers ua 
					ON ua.ConsentId = c.Id AND ua.UserId = @UserId AND ua.FlagDeleted = 0
				WHERE c.FlagDeleted = 0 AND c.QuestionnaireId=@QuestionnaireId";

			var consents = (await _connection.QueryAsync<ConsentRequest>(sql, new { UserId = userId, QuestionnaireId = questionnarieId })).ToList();

			return consents;
		}
		public async Task<bool> UpdateConsentAnswerAsync(int userId, List<UserConsentAnswer> consents)
		{
			using var _connection = _dbContext.CreateConnection();
			var sql = @"IF EXISTS (SELECT 1 FROM userconsentanswers WHERE ConsentId = @ConsentId AND UserId = @UserId)
				BEGIN
					UPDATE userconsentanswers 
					SET Answer = @Answer, UpdatedOn = GETDATE(), UpdatedBy = @UserId, FlagDeleted = 0
					WHERE ConsentId = @ConsentId AND UserId = @UserId
				END
				ELSE
				BEGIN
					INSERT INTO userconsentanswers (ConsentId, UserId, Answer, FlagDeleted, CreatedOn, CreatedBy)
					VALUES (@ConsentId, @UserId, @Answer, 0, GETDATE(), @UserId)
				END";

			int rowsAffected = 0;

			foreach (var item in consents)
			{
				var result = await _connection.ExecuteAsync(sql, new
				{
					ConsentId = item.ConsentId,
					UserId = userId,
					Answer = item.Answer
				});

				rowsAffected += result;
			}

			return rowsAffected > 0;
		}
	}
}
