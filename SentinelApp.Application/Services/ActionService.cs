using Microsoft.Extensions.Configuration;
using SentinelApp.Application.Interfaces;
using SentinelApp.Domain.Entities;
using SentinelApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentinelApp.Application.Services
{
	public class ActionService:IActionService
	{
		private readonly IMedicalHistory _medicalHistory;
		private readonly IConfiguration _config;
		private readonly ICurrentUserService _currentUserService;
		private readonly int _userId;
		private readonly int _questionnarieId;
		private readonly int _languageId;
		private readonly DateTime? _dateOfBirth;
		private readonly DateTime? _createdOn;
		public ActionService(IConfiguration config, ICurrentUserService currentUserService, IMedicalHistory medicalHistory)
		{
			_medicalHistory = medicalHistory;
			_config = config;
			_currentUserService = currentUserService;
			_userId = int.TryParse(_currentUserService.UserId, out var id) ? id : 0;
			_questionnarieId = _currentUserService.QuestionnaireId;
			_languageId = _currentUserService.PreferredLanguageId;
			_createdOn = _currentUserService.CreatedOn;
			_dateOfBirth = _currentUserService.DateOfBirth;
		}
		public async Task ProcessBreastSelfExamination(List<QuestionnaireAnswer> questionnaireAnswers)
		{
			if (questionnaireAnswers != null && questionnaireAnswers.Count > 0)
			{
				var mainQue = questionnaireAnswers.Where(f => f.IsMain).FirstOrDefault();
				if (mainQue == null)
					return;
				var colorcode = string.Empty;
				var status = string.Empty;
				var age20Date = _dateOfBirth.Value.AddYears(20);
				DateTime? dueDate = null;
				var today = DateTime.Now;
				var due4Week = today.AddDays(28);
				if (mainQue.SelectedAnswer == "Yes")
				{

				}
				if (mainQue.SelectedAnswer == "No")
				{
					var childQ = questionnaireAnswers.FirstOrDefault(x => x.QuestionCode == "BREAST_SELF_EXAM_ACTION");
					if (childQ != null)
					{
						if (childQ.OptionValue == "MASTECTOMY")
						{
							status = "Completed";
							dueDate = null;
							colorcode = "#3380FF";
						}
						else if (childQ.OptionValue == "PLAN_DISCUSS")
						{
							dueDate = new[]
							{
								age20Date,
								_createdOn,
							}.Max();
						}
						else if (childQ.OptionValue == "NO_PLAN")
						{
							dueDate = new[]
							{
								age20Date,
								_createdOn,
							}.Max();
						}
					}
				}
				colorcode = dueDate.HasValue && dueDate.Value.Date >= today && dueDate.Value.Date <= due4Week ? "#F94848" : "#3380FF";
				var action = new UserActionMaster
				{
					DueDate = dueDate,
					ColorCode = colorcode,
					Status = status,
					SortOrder = 6,
					QuestionId = mainQue.QuestionId,
					UserId = mainQue.UserId,
					CreatedBy = _userId,
					UpdatedBy = _userId
				};
				await _medicalHistory.UpsertUserActionAsync(action);
			}
		}
		public async Task ProcessMRIScan(List<QuestionnaireAnswer> questionnaireAnswers)
		{
			if (questionnaireAnswers != null && questionnaireAnswers.Count > 0)
			{
				var mainQue = questionnaireAnswers.Where(f => f.IsMain).FirstOrDefault();
				if (mainQue == null)
					return;
				var latestMRIDate = questionnaireAnswers.Where(x => x.QuestionCode == "MRI_HISTORY" && !string.IsNullOrEmpty(x.AnswerDate)).Select(s => DateTime.TryParse(s.AnswerDate, out var date) ? date : (DateTime?)null).Where(a => a.HasValue).OrderByDescending(x => x.Value).FirstOrDefault();
				var mriDueDate = latestMRIDate.HasValue ? latestMRIDate.Value.AddYears(1) : DateTime.MinValue;
				var age25Date = _dateOfBirth.Value.AddYears(25);
				DateTime? dueDate = null;
				var today = DateTime.Now;
				var due4Week = today.AddDays(28);

				if (mainQue.SelectedAnswer == "Yes")
				{
					dueDate = new[]
					{
						age25Date,
						mriDueDate,
						_createdOn,
					}.Max();
				}
				if (mainQue.SelectedAnswer == "No")
				{
					dueDate = new[]
					{
						age25Date,
						_createdOn,
					}.Max();
				}
				var colorcode = dueDate.HasValue && dueDate.Value.Date >= today && dueDate.Value.Date <= due4Week ? "#F94848" : "#3380FF";
				var action = new UserActionMaster
				{
					DueDate = dueDate,
					ColorCode = colorcode,
					SortOrder = 2,
					QuestionId = mainQue.QuestionId,
					UserId = mainQue.UserId,
					CreatedBy = _userId,
					UpdatedBy = _userId
				};
				await _medicalHistory.UpsertUserActionAsync(action);
			}
		}
		public async Task ProcessMamogramMRI(List<QuestionnaireAnswer> questionnaireAnswers)
		{
			if (questionnaireAnswers != null && questionnaireAnswers.Count > 0)
			{
				var mainQue = questionnaireAnswers.Where(f => f.IsMain).FirstOrDefault();
				if (mainQue == null)
					return;
				var colorcode = string.Empty;
				var status = string.Empty;
				var age25Date = _dateOfBirth.Value.AddYears(25);
				DateTime? dueDate = null;
				var today = DateTime.Now;
				var due4Week = today.AddDays(28);
				if (mainQue.SelectedAnswer == "Yes")
				{
					dueDate = new[]
					{
						age25Date,
						_createdOn,
					}.Max();
				}
				if (mainQue.SelectedAnswer == "No")
				{
					status = "Completed";
					dueDate = null;
				}
				colorcode = dueDate.HasValue && dueDate.Value.Date >= today && dueDate.Value.Date <= due4Week ? "#F94848" : "#3380FF";
				var action = new UserActionMaster
				{
					DueDate = dueDate,
					ColorCode = colorcode,
					Status = status,
					SortOrder = 3,
					QuestionId = mainQue.QuestionId,
					UserId = mainQue.UserId,
					CreatedBy = _userId,
					UpdatedBy = _userId
				};
				await _medicalHistory.UpsertUserActionAsync(action);
			}
		}
		public async Task ProcessMRIMamogram(List<QuestionnaireAnswer> questionnaireAnswers)
		{
			if (questionnaireAnswers != null && questionnaireAnswers.Count > 0)
			{
				var mainQue = questionnaireAnswers.Where(f => f.IsMain).FirstOrDefault();
				if (mainQue == null)
					return;
				var colorcode = string.Empty;
				var status = string.Empty;
				var age25Date = _dateOfBirth.Value.AddYears(25);
				DateTime? dueDate = null;
				var today = DateTime.Now;
				var due4Week = today.AddDays(28);
				if (mainQue.SelectedAnswer == "Yes")
				{
					dueDate = new[]
					{
						age25Date,
						_createdOn,
					}.Max();
				}
				if (mainQue.SelectedAnswer == "No")
				{
					status = "Completed";
					dueDate = null;
				}
				colorcode = dueDate.HasValue && dueDate.Value.Date >= today && dueDate.Value.Date <= due4Week ? "#F94848" : "#3380FF";
				var action = new UserActionMaster
				{
					DueDate = dueDate,
					ColorCode = colorcode,
					Status = status,
					SortOrder = 4,
					QuestionId = mainQue.QuestionId,
					UserId = mainQue.UserId,
					CreatedBy = _userId,
					UpdatedBy = _userId
				};
				await _medicalHistory.UpsertUserActionAsync(action);
			}
		}
		public async Task ProcessRedFlag(List<QuestionnaireAnswer> questionnaireAnswers)
		{
			if (questionnaireAnswers != null && questionnaireAnswers.Count > 0)
			{
				var mainQue = questionnaireAnswers.Where(f => f.IsMain).FirstOrDefault();
				if (mainQue == null)
					return;
				var colorcode = string.Empty;
				var status = string.Empty;
				var age18Date = _dateOfBirth.Value.AddYears(18);
				DateTime? dueDate = null;
				var today = DateTime.Now;
				var due4Week = today.AddDays(28);

				dueDate = new[]
				{
					age18Date,
					_createdOn,
				}.Max();
				colorcode = dueDate.HasValue && dueDate.Value.Date >= today && dueDate.Value.Date <= due4Week ? "#F94848" : "#3380FF";
				var action = new UserActionMaster
				{
					DueDate = dueDate,
					ColorCode = colorcode,
					Status = status,
					SortOrder = 5,
					QuestionId = mainQue.QuestionId,
					UserId = mainQue.UserId,
					CreatedBy = _userId,
					UpdatedBy = _userId
				};
				await _medicalHistory.UpsertUserActionAsync(action);
			}
		}
		public async Task ProcessRiskReducingMastectomy(List<QuestionnaireAnswer> questionnaireAnswers)
		{
			if (questionnaireAnswers != null && questionnaireAnswers.Count > 0)
			{
				var mainQue = questionnaireAnswers.Where(f => f.IsMain).FirstOrDefault();
				if (mainQue == null)
					return;
				var colorcode = string.Empty;
				var status = string.Empty;
				var age25Date = _dateOfBirth.Value.AddYears(25);
				DateTime? dueDate = null;
				var today = DateTime.Now;
				var due4Week = today.AddDays(28);
				if (mainQue.SelectedAnswer == "Yes")
				{
					status = "Completed";
				}
				if (mainQue.SelectedAnswer == "No")
				{
					var childQ = questionnaireAnswers.FirstOrDefault(x => x.QuestionCode == "MASTECTOMY_OUTCOME_NO");
					if (childQ != null)
					{
						if (childQ.OptionValue == "PLAN_DISCUSS")
						{
							dueDate = new[]
							{
								age25Date,
								_createdOn,
							}.Max();
						}
						else if (childQ.OptionValue == "NO_PLAN")
						{
							status = "Completed";
							dueDate = null;
						}
						else if (childQ.OptionValue == "NOT_CONSIDERED")
						{
							dueDate = new[]
							{
								age25Date,
								_createdOn,
							}.Max();
						}
					}
				}
				colorcode = dueDate.HasValue && dueDate.Value.Date >= today && dueDate.Value.Date <= due4Week ? "#F94848" : "#3380FF";
				var action = new UserActionMaster
				{
					DueDate = dueDate,
					ColorCode = colorcode,
					Status = status,
					SortOrder = 6,
					QuestionId = mainQue.QuestionId,
					UserId = mainQue.UserId,
					CreatedBy = _userId,
					UpdatedBy = _userId
				};
				await _medicalHistory.UpsertUserActionAsync(action);
			}
		}
		public async Task ProcessRiskReducingSalpingo(List<QuestionnaireAnswer> questionnaireAnswers)
		{
			if (questionnaireAnswers != null && questionnaireAnswers.Count > 0)
			{
				var mainQue = questionnaireAnswers.Where(f => f.IsMain).FirstOrDefault();
				if (mainQue == null)
					return;
				var colorcode = string.Empty;
				var status = string.Empty;
				var age25Date = _dateOfBirth.Value.AddYears(25);
				DateTime? dueDate = null;
				var today = DateTime.Now;
				var due4Week = today.AddDays(28);
				if (mainQue.SelectedAnswer == "Yes")
				{
					status = "Completed";
					var childQ = questionnaireAnswers.FirstOrDefault(x => x.QuestionCode == "SALPINGO_OUTCOME");
					if (childQ != null)
					{
						if (childQ.OptionValue == "HAD_OPERATION")
						{
							status = "Completed";
						}
					}
				}
				if (mainQue.SelectedAnswer == "No")
				{
					var childQ = questionnaireAnswers.FirstOrDefault(x => x.QuestionCode == "SALPINGO_OUTCOME_NO");
					if (childQ != null)
					{
						if (childQ.OptionValue == "PLAN_DISCUSS")
						{
							dueDate = new[]
							{
								age25Date,
								_createdOn,
							}.Max();
						}
						else if (childQ.OptionValue == "NO_PLAN")
						{
							status = "Completed";
							dueDate = null;
						}
						else if (childQ.OptionValue == "NOT_CONSIDERED")
						{
							dueDate = new[]
							{
								age25Date,
								_createdOn,
							}.Max();
						}
					}
				}
				colorcode = dueDate.HasValue && dueDate.Value.Date >= today && dueDate.Value.Date <= due4Week ? "#F94848" : "#3380FF";
				var action = new UserActionMaster
				{
					DueDate = dueDate,
					ColorCode = colorcode,
					Status = status,
					SortOrder = 7,
					QuestionId = mainQue.QuestionId,
					UserId = mainQue.UserId,
					CreatedBy = _userId,
					UpdatedBy = _userId
				};
				await _medicalHistory.UpsertUserActionAsync(action);
			}
		}
		public async Task ProcessTamoxifen(List<QuestionnaireAnswer> questionnaireAnswers)
		{
			if (questionnaireAnswers != null && questionnaireAnswers.Count > 0)
			{
				var mainQue = questionnaireAnswers.Where(f => f.IsMain).FirstOrDefault();
				if (mainQue == null)
					return;
				var colorcode = string.Empty;
				var status = string.Empty;
				var age25Date = _dateOfBirth.Value.AddYears(25);
				DateTime? dueDate = null;
				var today = DateTime.Now;
				var due4Week = today.AddDays(28);
				if (mainQue.SelectedAnswer == "Yes")
				{
					dueDate = new[]
					{
						age25Date,
						_createdOn,
					}.Max();
				}
				colorcode = dueDate.HasValue && dueDate.Value.Date >= today && dueDate.Value.Date <= due4Week ? "#F94848" : "#3380FF";
				var action = new UserActionMaster
				{
					DueDate = dueDate,
					ColorCode = colorcode,
					Status = status,
					SortOrder = 8,
					QuestionId = mainQue.QuestionId,
					UserId = mainQue.UserId,
					CreatedBy = _userId,
					UpdatedBy = _userId
				};
				await _medicalHistory.UpsertUserActionAsync(action);
			}
		}
	}
}
