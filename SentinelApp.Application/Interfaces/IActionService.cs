using SentinelApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentinelApp.Application.Interfaces
{
	public interface IActionService
	{
		Task ProcessBreastSelfExamination(List<QuestionnaireAnswer> questionnaireAnswers);
		Task ProcessMRIScan(List<QuestionnaireAnswer> questionnaireAnswers);
		Task ProcessMamogramMRI(List<QuestionnaireAnswer> questionnaireAnswers);
		Task ProcessMRIMamogram(List<QuestionnaireAnswer> questionnaireAnswers);
		Task ProcessRedFlag(List<QuestionnaireAnswer> questionnaireAnswers);
		Task ProcessRiskReducingMastectomy(List<QuestionnaireAnswer> questionnaireAnswers);
		Task ProcessRiskReducingSalpingo(List<QuestionnaireAnswer> questionnaireAnswers);
		Task ProcessTamoxifen(List<QuestionnaireAnswer> questionnaireAnswers);
	}
}
