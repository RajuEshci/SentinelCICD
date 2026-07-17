using Dapper;
using SentinelApp.Domain.Entities;
using SentinelApp.Domain.Interfaces;
using SentinelApp.Persistence.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Transactions;

namespace SentinelApp.Persistence.Repositories
{
	public class MedicalHistoryRepository : IMedicalHistory
	{
		private readonly IDbContext _dbContext;

		public MedicalHistoryRepository(IDbContext dbContext)
		{
			_dbContext = dbContext;
		}
		public async Task<object?> GetQuestionnaireAsync(int questionnaireId, int languageId, int UserId)
		{
			using var _connection = _dbContext.CreateConnection();

			const string sql = @"DECLARE @json NVARCHAR(MAX),
				@Gender VARCHAR(5);

				SELECT @Gender = Gender
				FROM Users
				WHERE UserId = @UserId;

				

				SET @json =(SELECT
					qr.QuestionnaireId,
					qr.QuestionnaireName,
					JSON_QUERY
					(
						(
							SELECT
								gg.GroupId,
								gg.GroupName,
								gg.Description AS GroupDescription,
								gg.DisplayOrder AS GroupDisplayOrder,

								JSON_QUERY
								(
									(
										SELECT
											q.QuestionId,
											q.QuestionCode,
											qt.QuestionText,
											q.QuestionType,
											q.IsRequired,
											q.IsMain,
											q.DisplayOrder,

											JSON_QUERY(opt.OptionsJson) AS [options],
											JSON_QUERY(dep.DependenciesJson) AS [dependencies],
											JSON_QUERY(msg.MessagesJson) AS [messages],
											JSON_QUERY(lbl.LabelJson) AS [labels]

										FROM Questions q
										INNER JOIN QuestionTranslations qt
											ON qt.QuestionId = q.QuestionId
										   AND qt.LanguageId = @LanguageId AND q.Gender=@Gender

										OUTER APPLY
										(
											SELECT
											(
												SELECT
													qo.OptionId,
													qo.OptionValue,
													qot.OptionText,
													qo.DisplayOrder
												FROM QuestionOptions qo
												INNER JOIN QuestionOptionTranslations qot
													ON qot.OptionId = qo.OptionId
												   AND qot.LanguageId = @LanguageId
												WHERE qo.QuestionCode = q.QuestionCode
												ORDER BY qo.DisplayOrder
												FOR JSON PATH
											) AS OptionsJson
										) opt

										OUTER APPLY
										(
											SELECT
											(
												SELECT
													ql.LabelType,
													ql.LabelText
												FROM  QuestionLabelTranslations ql
												  WHERE ql.LanguageId = @LanguageId AND ql.QuestionId = q.QuestionId
												FOR JSON PATH
											) AS LabelJson
										) lbl

										OUTER APPLY
										(
											SELECT
											(
												SELECT
													qd.ParentOptionValue,
													qd.ChildQuestionCode
												FROM QuestionDependencies qd
												WHERE qd.ParentQuestionCode = q.QuestionCode
												FOR JSON PATH
											) AS DependenciesJson
										) dep

										OUTER APPLY
										(
											SELECT
											(
												SELECT
													qm.OptionValue,
													qmt.MessageText
												FROM QuestionMessages qm
												INNER JOIN QuestionMessageTranslations qmt
													ON qmt.MessageId = qm.MessageId
												   AND qmt.LanguageId = @LanguageId
												WHERE qm.QuestionCode = q.QuestionCode
												FOR JSON PATH
											) AS MessagesJson
										) msg

										WHERE q.GroupId = gg.GroupId
										  AND q.QuestionnaireId = qr.QuestionnaireId

										ORDER BY q.DisplayOrder
										FOR JSON PATH
									)
								) AS questions

							FROM QuestionGroups gg
							WHERE gg.GroupId IN
							(
								SELECT DISTINCT GroupId
								FROM Questions
								WHERE QuestionnaireId = qr.QuestionnaireId
							)
							ORDER BY gg.DisplayOrder
							FOR JSON PATH
						)
					) AS [groups]

				FROM Questionnaires qr
				WHERE qr.QuestionnaireId = @QuestionnaireId
				FOR JSON PATH, ROOT('questionnaires')); SELECT @json;";

			var json = await _connection.ExecuteScalarAsync<string>(sql, new
			{
				QuestionnaireId = questionnaireId,
				LanguageId = languageId,
				UserId = UserId
			});

			if (string.IsNullOrWhiteSpace(json))
				return null;

			return JsonSerializer.Deserialize<object>(json);
		}
		public async Task<bool> UpsertUserAnswerAsync(List<UserAnswerMaster> addAnswers, List<UserAnswerMaster> removeAnswers)
		{
			using var _connection = _dbContext.CreateConnection();
			_connection.Open();
			using var transaction = _connection.BeginTransaction();
			try
			{
				if (removeAnswers != null && removeAnswers.Count > 0)
				{
					var removeSql = @"
					UPDATE UserAnswers
					SET
						FlagDeleted = 1,
						DeletedOn = GETDATE(),
						UpdatedBy = @UpdatedBy,
						UpdatedOn = GETDATE()
						WHERE UserAnswerId = @UserAnswerId AND FlagDeleted = 0";
					foreach (var answer in removeAnswers)
					{
						await _connection.ExecuteAsync(removeSql, new { UserAnswerId = answer.UserAnswerId, UpdatedBy = answer.UpdatedBy }, transaction);
					}
				}
				var sql = @"IF EXISTS(SELECT 1 FROM UserAnswers WHERE UserAnswerId = @UserAnswerId AND UserId = @UserId AND QuestionId = @QuestionId AND FlagDeleted = 0)
				BEGIN
					UPDATE UserAnswers
					SET
						OptionId = @OptionId,
						Answer = @Answer,
						AnswerDate = @AnswerDate,
						UpdatedBy = @UpdatedBy,
						UpdatedOn = GETDATE()
					WHERE UserAnswerId = @UserAnswerId
					  AND UserId = @UserId
					  AND QuestionId = @QuestionId
					  AND FlagDeleted = 0;
				END
				ELSE
				BEGIN
					INSERT INTO UserAnswers
					(
						UserId,
						QuestionId,
						OptionId,
						Answer,
						AnswerDate,
						CreatedBy
					)
					VALUES
					(
						@UserId,
						@QuestionId,
						@OptionId,
						@Answer,
						@AnswerDate,
						@CreatedBy
					);
				END";
				foreach (var answer in addAnswers)
				{
					await _connection.ExecuteAsync(sql, answer, transaction);
				}

				transaction.Commit();
				return true;
			}
			catch (Exception ex)
			{
				transaction.Rollback();
				return false;
			}
		}
		public async Task<List<UserAnswerMaster>> GetUserAnswersAsync(int userId)
		{
			using var _connection = _dbContext.CreateConnection();
			var sql = @"select * from UserAnswers where UserId=@UserId and FlagDeleted=0 and IsActive=1 order by QuestionId";
			var result = (await _connection.QueryAsync<UserAnswerMaster>(sql, new { UserId = userId })).ToList();
			return result;
		}
		public async Task<IEnumerable<UserAnswerHistoryMaster>> GetAllAsync(UserAnswerHistoryFilter filter)
		{
			using var _connection = _dbContext.CreateConnection();
			var sql = @"SELECT uah.*
						FROM UserAnswerHistory uah
						WHERE uah.UserId = @UserId";

			if (filter.QuestionId > 0)
			{
				sql += @" AND
						(
							uah.QuestionId = @QuestionId

							OR EXISTS
							(
								SELECT 1
								FROM Questions qp
								INNER JOIN QuestionDependencies qd
									ON qd.ParentQuestionCode = qp.QuestionCode
								INNER JOIN Questions qc
									ON qc.QuestionCode = qd.ChildQuestionCode
								WHERE qp.QuestionId = @QuestionId
								  AND qc.QuestionId = uah.QuestionId
							)
						)";
			}
			if (!string.IsNullOrEmpty(filter.Status))
			{
				sql += " AND IsActive = @IsActive";
				filter.IsActive = filter.Status.ToLower() == "active" ? true : false;
			}
			var sortCol = !string.IsNullOrEmpty(filter.SortColumn) ? filter.SortColumn : "CreatedOn";
			var sortDirection = !string.IsNullOrEmpty(filter.SortDirection) ? filter.SortDirection : "DESC";
			sql += $" ORDER BY ActionDate DESC";

			sql += " OFFSET @Offset ROWS FETCH NEXT @PerPage ROWS ONLY";
			filter.Offset = (filter.PageNo - 1) * filter.PerPage;
			return (await _connection.QueryAsync<UserAnswerHistoryMaster>(sql, filter)).ToList();
		}
		public async Task<CardCount> GetAllCardCounts(UserAnswerHistoryFilter filter)
		{
			using var _connection = _dbContext.CreateConnection();
			var countSql = @"SELECT 
				COUNT(*) AS Total,
				SUM(CASE WHEN IsActive = 1 THEN 1 ELSE 0 END) AS ActiveCount,
				SUM(CASE WHEN IsActive = 0 THEN 1 ELSE 0 END) AS InactiveCount
				FROM UserAnswerHistory uah
				WHERE uah.UserId = @UserId";

			if (filter.QuestionId > 0)
			{
				countSql += @" AND
						(
							uah.QuestionId = @QuestionId

							OR EXISTS
							(
								SELECT 1
								FROM Questions qp
								INNER JOIN QuestionDependencies qd
									ON qd.ParentQuestionCode = qp.QuestionCode
								INNER JOIN Questions qc
									ON qc.QuestionCode = qd.ChildQuestionCode
								WHERE qp.QuestionId = @QuestionId
								  AND qc.QuestionId = uah.QuestionId
							)
						)";
			}
			if (!string.IsNullOrEmpty(filter.Status))
			{
				countSql += " AND IsActive = @IsActive";
				filter.IsActive = filter.Status.ToLower() == "active" ? true : false;
			}

			return await _connection.QueryFirstAsync<CardCount>(countSql, filter);
		}
		public async Task<List<QuestionnaireAnswer>> GetQuestionnaireAnswerAsync(QuestionnaireAnswer questionnaireAnswer)
		{
			using var _connection = _dbContext.CreateConnection();
			var sql = @"DECLARE @RootQuestionCode VARCHAR(100);
				;WITH ParentHierarchy AS
				(
					SELECT
						Q.QuestionCode,
						Q.IsMain
					FROM Questions Q
					WHERE Q.QuestionId in @QuestionIds
					  AND Q.FlagDeleted = 0

					UNION ALL

					SELECT
						PQ.QuestionCode,
						PQ.IsMain
					FROM ParentHierarchy PH
					INNER JOIN QuestionDependencies QD
						ON QD.ChildQuestionCode = PH.QuestionCode
					   AND QD.FlagDeleted = 0
					INNER JOIN Questions PQ
						ON PQ.QuestionCode = QD.ParentQuestionCode
					   AND PQ.FlagDeleted = 0
					WHERE PH.IsMain = 0
				)
				SELECT TOP (1)
					@RootQuestionCode = QuestionCode
				FROM ParentHierarchy
				WHERE IsMain = 1;

				;WITH QuestionTree AS
				(
					SELECT
						Q.QuestionId,
						Q.QuestionCode,
						Q.QuestionCode AS ParentQuestionCode,
						0 AS Level
					FROM Questions Q
					WHERE Q.QuestionCode = @RootQuestionCode
					  AND Q.FlagDeleted = 0

					UNION ALL

					SELECT
						CQ.QuestionId,
						CQ.QuestionCode,
						QT.ParentQuestionCode,
						QT.Level + 1
					FROM QuestionTree QT
					INNER JOIN QuestionDependencies QD
						ON QD.ParentQuestionCode = QT.QuestionCode
					   AND QD.FlagDeleted = 0
					INNER JOIN Questions CQ
						ON CQ.QuestionCode = QD.ChildQuestionCode
					   AND CQ.FlagDeleted = 0
				)

				SELECT

					  QT.ParentQuestionCode
					, QT.Level

					, Q.QuestionId
					, Q.QuestionCode
					, Q.QuestionType
					, Q.IsMain
					, Q.DisplayOrder

					, QTN.QuestionText
					, QTN.ActionName

					, UA.UserAnswerId
					, UA.UserId
					, UA.OptionId
					, UA.Answer
					, UA.AnswerDate

					, QO.OptionValue
					, QOT.OptionText

					, CASE
						  WHEN UA.OptionId IS NOT NULL
							  THEN QOT.OptionText

						  WHEN UA.Answer IS NOT NULL
							  THEN UA.Answer

						  WHEN UA.AnswerDate IS NOT NULL
							  THEN CONVERT(varchar(10),UA.AnswerDate,120)

						  ELSE NULL
					  END AS SelectedAnswer

				FROM QuestionTree QT

				INNER JOIN Questions Q
					ON QT.QuestionId = Q.QuestionId

				INNER JOIN QuestionTranslations QTN
					ON QTN.QuestionId = Q.QuestionId
				   AND QTN.LanguageId = @LanguageId
				   AND QTN.FlagDeleted = 0

				LEFT JOIN UserAnswers UA
					ON UA.QuestionId = Q.QuestionId
				   AND UA.UserId = @UserId
				   AND ISNULL(UA.FlagDeleted,0)=0

				LEFT JOIN QuestionOptions QO
					ON QO.OptionId = UA.OptionId
				   AND QO.FlagDeleted = 0

				LEFT JOIN QuestionOptionTranslations QOT
					ON QOT.OptionId = UA.OptionId
				   AND QOT.LanguageId = @LanguageId
				   AND QOT.FlagDeleted = 0

				ORDER BY
					Q.DisplayOrder;";
			var result = (await _connection.QueryAsync<QuestionnaireAnswer>(sql, questionnaireAnswer)).ToList();
			return result;
		}
		public async Task<bool> UpsertUserActionAsync(UserActionMaster userActionMaster)
		{
			using var _connection = _dbContext.CreateConnection();
			_connection.Open();
			using var transaction = _connection.BeginTransaction();
			try
			{
				var sql = @"IF EXISTS(SELECT 1 FROM Actions WHERE UserId = @UserId AND QuestionId = @QuestionId AND FlagDeleted = 0)
				BEGIN
					UPDATE Actions
					SET
						DueDate = @DueDate,
						Status = @Status,
						ColorCode = @ColorCode,
						Sortorder = @Sortorder,
						UpdatedBy = @UpdatedBy,
						UpdatedOn = GETDATE()
					WHERE UserId = @UserId
					  AND QuestionId = @QuestionId
					  AND FlagDeleted = 0;
				END
				ELSE
				BEGIN
					INSERT INTO Actions
					(
						DueDate,
						Status,
						ColorCode,
						Sortorder,
						UserId,
						QuestionId,
						CreatedBy
					)
					VALUES
					(
						@DueDate,
						@Status,
						@ColorCode,
						@Sortorder,
						@UserId,
						@QuestionId,
						@CreatedBy
					);
				END";
				var result = await _connection.ExecuteAsync(sql, userActionMaster, transaction);
				transaction.Commit();
				return true;
			}
			catch (Exception ex)
			{
				transaction.Rollback();
				return false;
			}
		}
		public async Task<List<UserActionMaster>> GetActionsAsync(int userId, int languageId)
		{
			using var _connection = _dbContext.CreateConnection();
			var sql = @"SELECT
					Q.QuestionId,
					QT.ActionName,
					A.ActionId,
					ISNULL(A.DueDate, U.CreatedOn) AS DueDate,
					A.Status,
					CASE
						WHEN ISNULL(A.DueDate, U.CreatedOn) <= DATEADD(DAY, 28, GETDATE())
							THEN '#F94848'
						ELSE '#3380FF'
					END AS ColorCode,
					@UserId AS UserId

				FROM Questions Q

				INNER JOIN QuestionTranslations QT
					ON QT.QuestionId = Q.QuestionId
				   AND QT.LanguageId = @LanguageId
				   AND QT.FlagDeleted = 0

				INNER JOIN Users U
					ON U.UserId = @UserId

				LEFT JOIN Actions A
					ON A.QuestionId = Q.QuestionId
				   AND A.UserId = @UserId
				   AND A.FlagDeleted = 0

				WHERE
					Q.FlagDeleted = 0
					AND ISNULL(QT.ActionName, '') <> ''

				ORDER BY
					CASE
						WHEN ISNULL(A.DueDate, U.CreatedOn) <= DATEADD(DAY, 28, GETDATE())
							THEN 1       -- Red first
						ELSE 2           -- Blue second
					END,
					ISNULL(A.DueDate, U.CreatedOn) DESC;";

			var consents = (await _connection.QueryAsync<UserActionMaster>(sql, new { UserId = userId, LanguageId = languageId })).ToList();

			return consents;
		}
		public async Task<List<UserPDFDataMaster>> GetUserQuestionsDataAsync(int userId, int languageId)
		{
			using var _connection = _dbContext.CreateConnection();
			var sql = @"SELECT
					UA.UserAnswerId,
					UA.QuestionId,
					Q.DisplayOrder,
					QT.QuestionText,
					UA.AnswerDate,
					Q.IsMain,

					CASE
						WHEN UA.OptionId IS NOT NULL
							THEN QOT.OptionText
						ELSE UA.Answer
					END AS Answer

				FROM UserAnswers UA

				INNER JOIN Questions Q
					ON UA.QuestionId = Q.QuestionId
					AND Q.FlagDeleted = 0

				INNER JOIN QuestionTranslations QT
					ON QT.QuestionId = Q.QuestionId
					AND QT.LanguageId = @LanguageId
					AND QT.FlagDeleted = 0

				LEFT JOIN QuestionOptionTranslations QOT
					ON UA.OptionId = QOT.OptionId
					AND QOT.LanguageId = @LanguageId
					AND QOT.FlagDeleted = 0

				WHERE
					UA.UserId = @UserId
					AND UA.FlagDeleted = 0

				ORDER BY
					Q.DisplayOrder,
					UA.UserAnswerId;";

			var data = (await _connection.QueryAsync<UserPDFDataMaster>(sql, new { UserId = userId, LanguageId = languageId })).ToList();

			return data;
		}
	}
}
