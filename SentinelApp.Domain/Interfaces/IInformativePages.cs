using SentinelApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentinelApp.Domain.Interfaces
{
	public interface IInformativePages:IRepository<InformativePagesMaster, InformativePagesFilter>
	{
		Task<InformativePagesMaster?> GetByPageCodeIdAsync(string pgCode, int? questionnaireId);
		Task<List<InformativePagesMaster>?> GetInformativePagesListAsync(int questionnaireId);
		Task<CardCount> GetAllCardCounts(InformativePagesFilter filter);
		Task<List<InformativePagesMaster>?> GetMenuListAsync(int questionnaireId, string gender);
	}
}
