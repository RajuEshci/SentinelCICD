using Microsoft.Extensions.Configuration;
using SentinelApp.Application.Core;
using SentinelApp.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentinelApp.Application.Interfaces
{
	public interface IConsentService
	{
		Task<ServiceResponse<List<ConsentResponseDto>>> GetConsentByUserId();
		Task<ServiceResponse<bool>> UpdateConsentAnswerAsync(List<ConsentUserRequestDto> consents);
	}
}
