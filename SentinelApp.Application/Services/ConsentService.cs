using Microsoft.Extensions.Configuration;
using SentinelApp.Application.Core;
using SentinelApp.Application.DTO;
using SentinelApp.Application.Interfaces;
using SentinelApp.Domain.Entities;
using SentinelApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace SentinelApp.Application.Services
{
	public class ConsentService : IConsentService
	{
		private readonly IConsent _consent;
		private readonly IConfiguration _config;
		private readonly ICurrentUserService _currentUserService;
		private readonly int _userId;
		private readonly int _questionnarieId;
		public ConsentService(IConfiguration config, ICurrentUserService currentUserService, ICheckDuplicateRepository checkDuplicateRepository, IConsent consent)
		{
			_config = config;
			_currentUserService = currentUserService;
			_userId = int.TryParse(_currentUserService.UserId, out var id) ? id : 0;
			_consent = consent;
			_questionnarieId = _currentUserService.QuestionnaireId;
		}
		public async Task<ServiceResponse<List<ConsentResponseDto>>> GetConsentByUserId()
		{
			return await ServiceResponseExceptionHandler.Handle<List<ConsentResponseDto>>(async () =>
			{
				if (_userId == 0)
					throw new ServiceResponseException(System.Net.HttpStatusCode.Conflict, "User Not Found");
				var configConsent = _config["RequiredConsentId"];
				var consents = await _consent.GetConsentsAsync(_userId, configConsent, _questionnarieId);
				var requiredIds = new HashSet<int>();

				if (consents == null)
					throw new ServiceResponseException(System.Net.HttpStatusCode.Conflict, "Data Not Found");

				if (!string.IsNullOrWhiteSpace(configConsent))
				{
					requiredIds = configConsent
						.Split(',', StringSplitOptions.RemoveEmptyEntries)
						.Select(x => int.TryParse(x.Trim(), out var id) ? id : (int?)null)
						.Where(x => x.HasValue)
						.Select(x => x.Value)
						.ToHashSet();
				}

				foreach (var consent in consents)
				{
					consent.Required = requiredIds.Contains(consent.ConsentId);
				}
				return consents.Select(MapToDto).ToList();
			}, "Consent retrieved successfully.");
		}
		public async Task<ServiceResponse<bool>> UpdateConsentAnswerAsync(List<ConsentUserRequestDto> consents)
		{
			return await ServiceResponseExceptionHandler.Handle<bool>(async () =>
			{
				if (consents == null || !consents.Any())
					throw new ServiceResponseException(System.Net.HttpStatusCode.BadRequest, "Consent data is required.");

				var consentEntities = consents.Select(x => new UserConsentAnswer
				{
					ConsentId = x.ConsentId,
					Answer = x.Answer
				}).ToList();

				return await _consent.UpdateConsentAnswerAsync(_userId, consentEntities);
			},"Consent updated successfully.");
		}

		private ConsentResponseDto MapToDto(ConsentRequest consentRequest)
		{
			return new ConsentResponseDto
			{
				ConsentId = consentRequest.ConsentId,
				Answer = consentRequest.Answer,
				Description = consentRequest.Description,
				LinkText = consentRequest.LinkText,
				Required = consentRequest.Required,
				QuestionnarieId = consentRequest.QuestionnarieId,
			};
		}
	}
}
