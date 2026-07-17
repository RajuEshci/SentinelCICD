using SentinelApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentinelApp.Domain.Interfaces
{
	public interface IConsent
	{
		Task<List<ConsentRequest>> GetConsentsAsync(int userId, string configConsent, int questionnarieId);
		Task<bool> UpdateConsentAnswerAsync(int userId, List<UserConsentAnswer> consents);
	}
}
