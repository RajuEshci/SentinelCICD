using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using Scriban;
using SentinelApp.Application.Core;
using SentinelApp.Application.DTO;
using SentinelApp.Application.Interfaces;
using SentinelApp.Domain.Entities;
using SentinelApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SentinelApp.Application.Services
{
	public class MedicalHistoryService : IMedicalHistoryService
	{
		private readonly IMedicalHistory _medicalHistory;
		private readonly IConfiguration _config;
		private readonly ICurrentUserService _currentUserService;
		private readonly int _userId;
		private readonly int _questionnarieId;
		private readonly int _languageId;
		private readonly IConsent _consentRepository;
		private readonly IConsentService _consentService;
		private readonly DateTime? _dateOfBirth;
		private readonly DateTime? _createdOn;
		private readonly IActionService _actionService;
		private readonly IPdfService _pdfService;
		private readonly Email _email;
		private readonly string _userEmail;
		private readonly IUserRepository _users;
		public MedicalHistoryService(IConfiguration config, ICurrentUserService currentUserService, ICheckDuplicateRepository checkDuplicateRepository, IMedicalHistory medicalHistory, IConsent consentRepository, IConsentService consentService, IActionService actionService, IPdfService pdfService, Email email, IUserRepository users)
		{
			_config = config;
			_currentUserService = currentUserService;
			_userId = int.TryParse(_currentUserService.UserId, out var id) ? id : 0;
			_medicalHistory = medicalHistory;
			_questionnarieId = _currentUserService.QuestionnaireId;
			_languageId = _currentUserService.PreferredLanguageId;
			_consentRepository = consentRepository;
			_consentService = consentService;
			_createdOn = _currentUserService.CreatedOn;
			_dateOfBirth = _currentUserService.DateOfBirth;
			_actionService = actionService;
			_pdfService = pdfService;
			_email = email;
			_userEmail = _currentUserService.Email;
			_users = users;
		}
		public async Task<ServiceResponse<object>> GetQuestionnaireAsync()
		{
			return await ServiceResponseExceptionHandler.Handle<object>(async () =>
			{
				var consents = await _consentService.GetConsentByUserId();
				var pendingRequiredConsents = consents.DataResult.Where(x => x.Required && !string.Equals(x.Answer, "Yes", StringComparison.OrdinalIgnoreCase)).ToList();

				if (pendingRequiredConsents.Any())
				{
					throw new ServiceResponseException(System.Net.HttpStatusCode.Conflict, $"To proceed, please review and accept the required consent. This is necessary to use all features.");
				}
				var data = await _medicalHistory.GetQuestionnaireAsync(_questionnarieId, _languageId != 0 ? _languageId : 1, _userId);

				if (data == null)
					throw new ServiceResponseException(System.Net.HttpStatusCode.Conflict, "Data Not Found");

				return data;
			}, "Data retrieved successfully.");
		}
		public async Task<ServiceResponse<bool>> UpsertUserAnswer(UpsertUserAnswerDto createUserAnswerDto)
		{
			return await ServiceResponseExceptionHandler.Handle<bool>(async () =>
			{
				var consents = await _consentService.GetConsentByUserId();
				var pendingRequiredConsents = consents.DataResult.Where(x => x.Required && !string.Equals(x.Answer, "Yes", StringComparison.OrdinalIgnoreCase)).ToList();

				if (pendingRequiredConsents.Any())
				{
					throw new ServiceResponseException(System.Net.HttpStatusCode.Conflict, $"To proceed, please review and accept the required consent. This is necessary to use all features.");
				}
				var addAnswers = createUserAnswerDto.Add.Select(s => new UserAnswerMaster
				{
					UserAnswerId = s.UserAnswerId,
					UserId = _userId,
					QuestionId = s.QuestionId,
					OptionId = s.OptionId,
					Answer = s.Answer,
					AnswerDate = s.AnswerDate,
					CreatedBy = _userId,
					UpdatedBy = s.UserAnswerId > 0 ? _userId : 0
				}).ToList();
				var removeAnswers = createUserAnswerDto.Remove.Select(s => new UserAnswerMaster
				{
					UserAnswerId = s.UserAnswerId,
				}).ToList();
				var uAnswer = await _medicalHistory.UpsertUserAnswerAsync(addAnswers, removeAnswers);
				if (uAnswer)
				{
					var qIds = createUserAnswerDto.Add.Select(s => s.QuestionId).ToList();

					var data = new QuestionnaireAnswer
					{
						UserId = _userId,
						QuestionIds = qIds,
						LanguageId = 1,
						//LanguageId = _languageId,
					};
					var qData = await _medicalHistory.GetQuestionnaireAnswerAsync(data);
					if (qData != null && qData.Count > 0)
					{
						await ProcessQuestionActions(qData);
					}
				}
				return uAnswer;
			}, "User medical history added successfully.");
		}
		public async Task<ServiceResponse<List<UserAnswerReponseDto>>> GetUserAnswersAsync()
		{
			return await ServiceResponseExceptionHandler.Handle<List<UserAnswerReponseDto>>(async () =>
			{
				var consents = await _consentService.GetConsentByUserId();
				var pendingRequiredConsents = consents.DataResult.Where(x => x.Required && !string.Equals(x.Answer, "Yes", StringComparison.OrdinalIgnoreCase)).ToList();

				if (pendingRequiredConsents.Any())
				{
					throw new ServiceResponseException(System.Net.HttpStatusCode.Conflict, $"To proceed, please review and accept the required consent. This is necessary to use all features.");
				}
				var userAnswerData = await _medicalHistory.GetUserAnswersAsync(_userId);

				if (userAnswerData?.Count() == 0)
					throw new ServiceResponseException(System.Net.HttpStatusCode.Conflict, "Data not found.");

				var userAnswerList = userAnswerData.Select(MapToDto).ToList();
				return userAnswerList;
			}, "Data retrieved successfully.");
		}
		public async Task<ServiceResponse<PaginatedResponseDto<UserAnswerHistoryDto>>> GetMedicalHistoryAsync(UserAnswerHistoryFilterDto filterDto)
		{
			return await ServiceResponseExceptionHandler.Handle<PaginatedResponseDto<UserAnswerHistoryDto>>(async () =>
			{
				var consents = await _consentService.GetConsentByUserId();
				var pendingRequiredConsents = consents.DataResult.Where(x => x.Required && !string.Equals(x.Answer, "Yes", StringComparison.OrdinalIgnoreCase)).ToList();

				if (pendingRequiredConsents.Any())
				{
					throw new ServiceResponseException(System.Net.HttpStatusCode.Conflict, $"To proceed, please review and accept the required consent. This is necessary to use all features.");
				}
				var filter = new UserAnswerHistoryFilter
				{
					UserId = _userId,
					QuestionId = filterDto.QuestionId,
					Status = filterDto.Status,
					SortColumn = filterDto.SortColumn,
					SortDirection = filterDto.SortDirection,
					PageNo = filterDto.Page,
					PerPage = filterDto.PerPage,
				};
				var cardCount = await _medicalHistory.GetAllCardCounts(filter);

				var pagesData = await _medicalHistory.GetAllAsync(filter);

				if (pagesData.Count() == 0)
					throw new ServiceResponseException(System.Net.HttpStatusCode.Conflict, "No data found.");

				var totalPages = (int)Math.Ceiling(cardCount.Total / (double)filterDto.PerPage);

				var currencyDtos = pagesData.Select(MapToHistoryDto).ToList();

				return new PaginatedResponseDto<UserAnswerHistoryDto>
				{
					Page = filterDto.Page,
					PerPage = filterDto.PerPage,
					Total = cardCount.Total,
					TotalPages = totalPages,
					Data = currencyDtos,
					CardCount = new CardCountDto
					{
						Total = cardCount.Total,
						ActiveCount = cardCount.ActiveCount,
						InactiveCount = cardCount.InactiveCount
					}
				};
			}, "Data retrieved successfully.");
		}
		public async Task<ServiceResponse<List<UserActionResponseDto>>> GetActionsByUserId()
		{
			return await ServiceResponseExceptionHandler.Handle<List<UserActionResponseDto>>(async () =>
			{
				if (_userId == 0)
					throw new ServiceResponseException(System.Net.HttpStatusCode.Conflict, "User Not Found");
				var actionsData = await _medicalHistory.GetActionsAsync(_userId, _languageId != 0 ? _languageId : 1);
				if (actionsData?.Count() == 0)
					throw new ServiceResponseException(System.Net.HttpStatusCode.Conflict, "Data not found.");
				var dueDate = _dateOfBirth.Value.AddYears(18) > _createdOn ? _dateOfBirth.Value.AddYears(18) : _createdOn;
				actionsData.Add(new UserActionMaster
				{
					ActionName = "Review Red Flags",
					DueDate = dueDate,
					ColorCode = dueDate <= DateTime.Today.AddDays(28) ? "#F94848" : "#3380FF",
					UserId = _userId
				});
				actionsData = actionsData.OrderBy(x => x.ColorCode == "#F94848" ? 0 : 1).ThenByDescending(x => x.DueDate).ToList();
				return actionsData.Select(MapToUserActionDto).ToList();
			}, "Actions retrieved successfully.");
		}
		public async Task<ServiceResponse<string>> GenerateMedicalHistoryPdf()
		{
			return await ServiceResponseExceptionHandler.Handle<string>(async () =>
			{
				var result = await _medicalHistory.GetUserQuestionsDataAsync(_userId, _languageId != 0 ? _languageId : 1);

				if (result == null || !result.Any())
				{
					var mhEmptyhtmlTemplate = LoadEmptyPDFHtmlTemplate("en-GB");
					var mhEmptytemplate = Template.Parse(mhEmptyhtmlTemplate);
					var mhEmptyrenderedHtml = mhEmptytemplate.Render(new { year = DateTime.Now.Year });
					throw new ServiceResponseException(HttpStatusCode.NotFound, "No detail for medical history available.");
				}

				var model = new MedicalHistoryPDFDto();

				model.Questions = result
					.GroupBy(x => new
					{
						x.QuestionId,
						x.QuestionText,
						x.DisplayOrder
					})
					.OrderBy(x => x.Key.DisplayOrder)
					.Select(g => new MedicalHistoryQuestionDto
					{
						Question = g.Key.QuestionText,

						Answers = g.Select(x =>
							{
								if (x.AnswerDate.HasValue && !string.IsNullOrWhiteSpace(x.Answer))
									return $"{x.AnswerDate:dd/MM/yyyy} - {x.Answer}";

								if (x.AnswerDate.HasValue)
									return x.AnswerDate.Value.ToString("dd/MM/yyyy");

								return x.Answer;
							})
							.Where(x => !string.IsNullOrWhiteSpace(x))
							.ToList()
					})
					.ToList();

				var htmlTemplate = LoadPDFHtmlTemplate("en-GB");
				var template = Template.Parse(htmlTemplate);
				var renderedHtml = template.Render(model, member => member.Name);

				var pdfBytes = await _pdfService.GeneratePdfAsync(renderedHtml);
				List<MailAttachmentDto>? mailAttachments = null;
				if (pdfBytes != null && pdfBytes.Length > 0)
				{
					var stream = new MemoryStream(pdfBytes);
					mailAttachments = new List<MailAttachmentDto>
					{
						new MailAttachmentDto
						{
							FileBytes = pdfBytes,
							FileName = "MedicalHistory.pdf",
							ContentType = "application/pdf"
						}
					};
				}
				var emailTemplate = LoadPDFMailBodyHtmlTemplate("en-GB");
				var etemplate = Template.Parse(emailTemplate);
				var emailrenderedHtml = etemplate.Render(new { year = DateTime.Now.Year });
				_email.SendEmailOnPdfExport(_userEmail, emailrenderedHtml, mailAttachments);
				#region deactivate user
				await _users.DeactivateUserAsync(_userId,_userEmail);
				#endregion
				return "We will email a copy of your data to your email address registered in the app. We will then close your record. If you wish to reopen your record you will need to re-register in the app and add your data again.";
			});
		}

		private async Task ProcessQuestionActions(List<QuestionnaireAnswer> questionnaireAnswers)
		{
			var qCode = questionnaireAnswers.FirstOrDefault(f => f.IsMain);
			if (qCode == null)
				return;
			switch (qCode.QuestionCode)
			{
				case "BREAST_SELF_EXAM":
					await _actionService.ProcessBreastSelfExamination(questionnaireAnswers);
					break;
				case "MRI_SCAN":
					await _actionService.ProcessMRIScan(questionnaireAnswers);
					break;
				case "MAMMOGRAM_WITH_MRI":
					await _actionService.ProcessMamogramMRI(questionnaireAnswers);
					break;
				case "MRI_WITH_MAMMOGRAM":
					await _actionService.ProcessMRIMamogram(questionnaireAnswers);
					break;
				case "MASTECTOMY_DISCUSSION":
					await _actionService.ProcessRiskReducingMastectomy(questionnaireAnswers);
					break;
				case "SALPINGO_DISCUSSION":
					await _actionService.ProcessRiskReducingSalpingo(questionnaireAnswers);
					break;
				case "TAMOXIFEN_DISCUSSION":
					await _actionService.ProcessTamoxifen(questionnaireAnswers);
					break;
			}
		}

		private UserAnswerReponseDto MapToDto(UserAnswerMaster userAnswerMaster)
		{
			return new UserAnswerReponseDto
			{
				UserAnswerId = userAnswerMaster.UserAnswerId,
				UserId = userAnswerMaster.UserId,
				QuestionId = userAnswerMaster.QuestionId,
				OptionId = userAnswerMaster.OptionId,
				Answer = userAnswerMaster.Answer,
				AnswerDate = userAnswerMaster.AnswerDate,
			};
		}
		private UserAnswerHistoryDto MapToHistoryDto(UserAnswerHistoryMaster userAnswerHistoryMaster)
		{
			return new UserAnswerHistoryDto
			{
				UserAnswerHistoryId = userAnswerHistoryMaster.UserAnswerHistoryId,
				UserAnswerId = userAnswerHistoryMaster.UserAnswerId,
				UserId = userAnswerHistoryMaster.UserId,
				QuestionId = userAnswerHistoryMaster.QuestionId,
				OptionId = userAnswerHistoryMaster.OptionId,
				Answer = userAnswerHistoryMaster.Answer,
				AnswerDate = userAnswerHistoryMaster.AnswerDate,
				ActionType = userAnswerHistoryMaster.ActionType,
				ActionDate = userAnswerHistoryMaster.ActionDate,
				CreatedOn = userAnswerHistoryMaster.CreatedOn,
			};
		}
		private UserActionResponseDto MapToUserActionDto(UserActionMaster userActionMaster)
		{
			return new UserActionResponseDto
			{
				ActionId = userActionMaster.ActionId,
				ActionName = userActionMaster.ActionName,
				DueDate = userActionMaster.DueDate,
				Status = userActionMaster.Status,
				ColorCode = userActionMaster.ColorCode,
				//SortOrder = userActionMaster.SortOrder,
				QuestionId = userActionMaster.QuestionId,
				UserId = userActionMaster.UserId
			};
		}
		private string LoadPDFMailBodyHtmlTemplate(string lCode)
		{
			var htmlFilePath = Path.Combine(AppContext.BaseDirectory, "Html", $"MedicalHistoryDataMail_{lCode}.html");
			if (!System.IO.File.Exists(htmlFilePath))
			{
				htmlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Html", $"MedicalHistoryDataMail_en-GB.html");
			}
			return System.IO.File.ReadAllText(htmlFilePath);
		}
		private string LoadPDFHtmlTemplate(string lCode)
		{
			var htmlFilePath = Path.Combine(AppContext.BaseDirectory, "Html", $"MedicalHistoryReport_{lCode}.html");
			if (!System.IO.File.Exists(htmlFilePath))
			{
				htmlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Html", $"MedicalHistoryReport_en-GB.html");
			}
			return System.IO.File.ReadAllText(htmlFilePath);
		}
		private string LoadEmptyPDFHtmlTemplate(string lCode)
		{
			var htmlFilePath = Path.Combine(AppContext.BaseDirectory, "Html", $"MedicalHistoryEmptydataMail_{lCode}.html");
			if (!System.IO.File.Exists(htmlFilePath))
			{
				htmlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Html", $"MedicalHistoryEmptydataMail_en-GB.html");
			}
			return System.IO.File.ReadAllText(htmlFilePath);
		}
	}
}
