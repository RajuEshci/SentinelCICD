using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentinelApp.Application.DTO
{
	public class MailAttachmentDto
	{
		public byte[] FileBytes { get; set; }
		public string FileName { get; set; }
		public string ContentType { get; set; }
	}
}
