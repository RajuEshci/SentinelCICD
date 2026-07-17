using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentinelApp.API.Middleware
{
	public interface ITranslationProvider
	{
		string Get(string key);
		string Get(string key, string langCode);
	}
}
