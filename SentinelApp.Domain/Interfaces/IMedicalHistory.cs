using SentinelApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SentinelApp.Domain.Interfaces
{
	public interface IMedicalHistory
	{
		Task<object?> GetQuestionnaireAsync(int questionnaireId, int languageId, int UserId);
		Task<bool> UpsertUserAnswerAsync(List<UserAnswerMaster> addAnswers, List<UserAnswerMaster> removeAnswers);
		Task<List<UserAnswerMaster>> GetUserAnswersAsync(int userId);
		Task<IEnumerable<UserAnswerHistoryMaster>> GetAllAsync(UserAnswerHistoryFilter filter);
		Task<CardCount> GetAllCardCounts(UserAnswerHistoryFilter filter);
		Task<List<QuestionnaireAnswer>> GetQuestionnaireAnswerAsync(QuestionnaireAnswer questionnaireAnswer);
		Task<bool> UpsertUserActionAsync(UserActionMaster userActionMaster);
		Task<List<UserActionMaster>> GetActionsAsync(int userId, int languageId);
		Task<List<UserPDFDataMaster>> GetUserQuestionsDataAsync(int userId, int languageId);
	}
}
