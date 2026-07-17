using iText.Html2pdf;
using SentinelApp.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentinelApp.Application.Services
{
	public class PdfService : IPdfService
	{
		public async Task<byte[]> GeneratePdfAsync(string html)
		{
			using var memoryStream = new MemoryStream();

			HtmlConverter.ConvertToPdf(html, memoryStream);

			return await Task.FromResult(memoryStream.ToArray());
		}
	}
}
