using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentinelApp.Application.Interfaces
{
	public interface IPdfService
	{
		Task<byte[]> GeneratePdfAsync(string html);
	}
}
