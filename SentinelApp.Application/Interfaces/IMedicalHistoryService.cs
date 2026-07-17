using SentinelApp.Application.Core;
using SentinelApp.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SentinelApp.Application.Interfaces
{
	public interface IMedicalHistoryService
	{
		Task<ServiceResponse<object>> GetQuestionnaireAsync();
		Task<ServiceResponse<bool>> UpsertUserAnswer(UpsertUserAnswerDto createUserAnswerDto);
		Task<ServiceResponse<List<UserAnswerReponseDto>>> GetUserAnswersAsync();
		Task<ServiceResponse<PaginatedResponseDto<UserAnswerHistoryDto>>> GetMedicalHistoryAsync(UserAnswerHistoryFilterDto filterDto);
		Task<ServiceResponse<List<UserActionResponseDto>>> GetActionsByUserId();
		Task<ServiceResponse<string>> GenerateMedicalHistoryPdf();
	}
}
