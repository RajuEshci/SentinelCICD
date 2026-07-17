using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using SentinelApp.Application.DTO;

namespace SentinelApp.Application.Core
{
	public class Email
	{
		private readonly IConfiguration _config;
		Regex rgEmail = new Regex(@"[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}");
		private readonly ILogger<Email> _logger;
		public Email(IConfiguration config, ILogger<Email> logger)
		{
			_config = config;
			_logger = logger;
		}
		public void SendMail(
		   string toEmail,
		   string subject,
		   string htmlBody,
		   List<MailAttachmentDto>? attachments = null)
		{
			string provider = _config["EmailProvider"];

			if (provider == "GRAPH")
			{
				SendUsingMicrosoftGraph(toEmail, subject, htmlBody, attachments);
			}
			else
			{
				SendUsingSmtp(toEmail, subject, htmlBody, attachments);
			}
		}

		// ===================== SMTP (GMAIL) =====================
		private void SendUsingSmtp(string toEmail, string subject, string htmlBody, List<MailAttachmentDto>? attachments)
		{
			try
			{
				using var message = new MailMessage
				{
					From = new MailAddress(
						_config["SMTP:FromEmail"],
						_config["SMTP:FromName"]
					),
					Subject = subject,
					Body = htmlBody,
					IsBodyHtml = true
				};

				// Add recipients
				foreach (var mail in toEmail.Split(',', StringSplitOptions.RemoveEmptyEntries))
					message.To.Add(mail.Trim());

				var (processedHtml, linkedResources) = ProcessBase64Images(htmlBody);

				AlternateView htmlView = AlternateView.CreateAlternateViewFromString(
					processedHtml,
					null,
					MediaTypeNames.Text.Html
				);

				// Add linked resources for inline images
				foreach (var resource in linkedResources)
				{
					htmlView.LinkedResources.Add(resource);
				}

				message.AlternateViews.Add(htmlView);

				// Add attachments (with proper disposal)
				if (attachments != null)
				{
					foreach (var a in attachments)
					{
						var stream = new MemoryStream(a.FileBytes);
						message.Attachments.Add(
							new Attachment(stream, a.FileName, a.ContentType)
						);
					}
				}

				// Configure and send
				using var smtp = new SmtpClient(
					 _config["SMTP:Host"],
					 int.Parse(_config["SMTP:Port"]))
				{
					EnableSsl = bool.Parse(_config["SMTP:EnableSsl"]),
					DeliveryMethod = SmtpDeliveryMethod.Network,
					UseDefaultCredentials = false,
					Credentials = new NetworkCredential(
						 _config["SMTP:UserName"],
						 _config["SMTP:Password"]),
					Timeout = 30000 // 30 seconds timeout
				};

				smtp.Send(message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "SMTP mail failed to {ToEmail}", toEmail);
				throw;
			}
		}

		// ===================== MICROSOFT GRAPH =====================
		private void SendUsingMicrosoftGraph(string toEmail, string subject, string htmlBody, List<MailAttachmentDto>? attachments)
		{
			try
			{
				string token = GetGraphAccessToken().Result;

				var toRecipients = toEmail
					.Split(',', StringSplitOptions.RemoveEmptyEntries)
					.Select(e => new
					{
						emailAddress = new { address = e.Trim() }
					}).ToList();

				// Process base64 images and convert to inline attachments
				var (processedHtml, inlineAttachments) = ProcessBase64ImagesForGraph(htmlBody);

				var graphAttachments = new List<Dictionary<string, object>>();

				// Add inline attachments from base64 images
				foreach (var inline in inlineAttachments)
				{
					graphAttachments.Add(new Dictionary<string, object>
					{
						{ "@odata.type", "#microsoft.graph.fileAttachment" },
						{ "name", inline.FileName },
						{ "contentType", inline.ContentType },
						{ "contentBytes", Convert.ToBase64String(inline.FileBytes) },
						{ "isInline", true },
						{ "contentId", inline.ContentId }
					});
				}

				// Add regular attachments
				if (attachments != null)
				{
					foreach (var a in attachments)
					{
						graphAttachments.Add(new Dictionary<string, object>
						{
							{ "@odata.type", "#microsoft.graph.fileAttachment" },
							{ "name", a.FileName },
							{ "contentType", a.ContentType },
							{ "contentBytes", Convert.ToBase64String(a.FileBytes) },
							{ "isInline", false }
						});
					}
				}

				var payload = new
				{
					message = new
					{
						subject = subject,
						body = new
						{
							contentType = "HTML",
							content = processedHtml  // HTML with CID references
						},
						toRecipients = toRecipients,
						attachments = graphAttachments
					},
					saveToSentItems = true
				};

				var json = JsonConvert.SerializeObject(payload);

				using var httpClient = new HttpClient();
				httpClient.DefaultRequestHeaders.Authorization =
					new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

				var url = $"https://graph.microsoft.com/v1.0/users/{_config["GraphEmail:FromUser"]}/sendMail";
				var content = new StringContent(json, Encoding.UTF8, "application/json");

				var response = httpClient.PostAsync(url, content).Result;

				if (!response.IsSuccessStatusCode)
				{
					var errorContent = response.Content.ReadAsStringAsync().Result;
					_logger.LogError("Graph API error: {StatusCode} - {Error}",
						response.StatusCode, errorContent);
					throw new HttpRequestException($"Graph API failed: {response.StatusCode}");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Graph mail failed");
				throw;
			}
		}

		private (string processedHtml, List<GraphInlineAttachment> inlineAttachments)
			ProcessBase64ImagesForGraph(string html)
		{
			var inlineAttachments = new List<GraphInlineAttachment>();
			var processedHtml = html;

			var pattern = @"src=""data:image/(?<type>[a-zA-Z]+);base64,(?<data>[^""]+)""";
			var matches = Regex.Matches(html, pattern);
			int counter = 0;

			foreach (Match match in matches)
			{
				if (match.Success)
				{
					var contentType = match.Groups["type"].Value;
					var base64Data = match.Groups["data"].Value;

					try
					{
						var imageBytes = Convert.FromBase64String(base64Data);
						var contentId = $"image_{Guid.NewGuid():N}";
						var fileName = $"image_{counter}.{contentType}";

						inlineAttachments.Add(new GraphInlineAttachment
						{
							FileBytes = imageBytes,
							ContentType = $"image/{contentType}",
							FileName = fileName,
							ContentId = contentId
						});

						// Replace with CID reference
						processedHtml = processedHtml.Replace(
							match.Value,
							$"src=\"cid:{contentId}\""
						);

						counter++;
					}
					catch (Exception ex)
					{
						_logger.LogWarning(ex, "Failed to process base64 image for Graph");
					}
				}
			}

			return (processedHtml, inlineAttachments);
		}

		// Helper class for Graph inline attachments
		public class GraphInlineAttachment
		{
			public byte[] FileBytes { get; set; }
			public string ContentType { get; set; }
			public string FileName { get; set; }
			public string ContentId { get; set; }
		}
		// ===================== GRAPH TOKEN =====================
		private async Task<string> GetGraphAccessToken()
		{
			var app = ConfidentialClientApplicationBuilder
				.Create(_config["GraphEmail:ClientId"])
				.WithClientSecret(_config["GraphEmail:ClientSecret"])
				.WithAuthority(
					$"{_config["GraphEmail:Authority"]}{_config["GraphEmail:TenantId"]}")
				.Build();

			var result = await app
				.AcquireTokenForClient(
					new[] { _config["GraphEmail:Scope"] })
				.ExecuteAsync();

			return result.AccessToken;
		}

		private (string processedHtml, List<LinkedResource> linkedResources) ProcessBase64Images(string html)
		{
			var linkedResources = new List<LinkedResource>();
			var processedHtml = html;

			// Regular expression to find base64 images
			var pattern = @"src=""data:image/(?<type>[a-zA-Z]+);base64,(?<data>[^""]+)""";
			var matches = Regex.Matches(html, pattern);
			int counter = 0;

			foreach (Match match in matches)
			{
				if (match.Success)
				{
					var contentType = match.Groups["type"].Value;
					var base64Data = match.Groups["data"].Value;

					try
					{
						// Convert base64 to bytes
						var imageBytes = Convert.FromBase64String(base64Data);

						// Create unique Content-ID
						var contentId = $"image_{Guid.NewGuid():N}";

						// Create linked resource
						var resource = new LinkedResource(
							new MemoryStream(imageBytes),
							$"image/{contentType}")
						{
							ContentId = contentId,
							TransferEncoding = TransferEncoding.Base64
						};

						linkedResources.Add(resource);

						// Replace base64 src with CID reference
						processedHtml = processedHtml.Replace(
							match.Value,
							$"src=\"cid:{contentId}\""
						);

						counter++;
					}
					catch (Exception ex)
					{
						_logger.LogWarning(ex, "Failed to process base64 image");
					}
				}
			}

			_logger.LogInformation("Processed {Count} base64 images for inline display", counter);
			return (processedHtml, linkedResources);
		}
		//      public void SendEmail(string toEmailId, string emailSubject, Dictionary<string, string> messageText, string htmlFilePath, string replyToEmailId = null)
		//{

		//	try
		//	{
		//		string fromEmailId;
		//		StreamReader sr = new StreamReader(htmlFilePath);

		//		string emailBody = sr.ReadToEnd();
		//		string emailUserName, emailPassward, fromName;

		//		foreach (KeyValuePair<string, string> msgParameter in messageText)
		//		{
		//			emailBody = emailBody.Replace(msgParameter.Key, msgParameter.Value);
		//		}

		//		fromEmailId = _config["FromEmail"];
		//		emailUserName = _config["EmailUserName"];
		//		emailPassward = _config["EmailPassward"];
		//		fromName = _config["FromName"];

		//		//Message settings
		//		MailMessage message = new MailMessage();
		//		message.From = new MailAddress(fromEmailId, fromName);
		//		foreach (var address in toEmailId.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries))
		//		{
		//			message.To.Add(address);
		//		}

		//		// Add Reply-To
		//		if (!string.IsNullOrEmpty(replyToEmailId))
		//		{
		//			foreach (var replyToAddress in replyToEmailId.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries))
		//			{
		//				message.ReplyToList.Add(new MailAddress(replyToAddress));
		//			}
		//		}
		//		string globalBcc = _config["GlobalBCC"];
		//		string excludedSubjects = _config["ExcludedEmailSubjects"];
		//		if (!string.IsNullOrEmpty(globalBcc))
		//		{
		//			if (!string.IsNullOrEmpty(excludedSubjects))
		//			{
		//				var excludedSubjectList = excludedSubjects.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
		//									  .Select(s => s.Trim());
		//				if (!excludedSubjectList.Contains(emailSubject))
		//				{
		//					foreach (var bccAddress in globalBcc.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries))
		//					{
		//						message.Bcc.Add(bccAddress.Trim());
		//					}
		//				}
		//			}
		//			else
		//			{
		//				foreach (var bccAddress in globalBcc.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries))
		//				{
		//					message.Bcc.Add(bccAddress.Trim());
		//				}
		//			}
		//		}

		//		message.Subject = emailSubject;

		//		// done HTML formatting in the next line to display my logo
		//		AlternateView av1 = AlternateView.CreateAlternateViewFromString(emailBody, null, MediaTypeNames.Text.Html);

		//		message.AlternateViews.Add(av1);
		//		message.IsBodyHtml = true;

		//		message.AlternateViews.Add(av1);
		//		//Smstp settings
		//		SmtpClient emailClient = new SmtpClient();
		//		emailClient.Host = _config["EmailHostName"];// "smtp.gmail.com";
		//		emailClient.Port = Convert.ToInt32(_config["EmailPort"]);// 587;
		//		emailClient.EnableSsl = true;
		//		System.Net.NetworkCredential SMTPUserInfo = new System.Net.NetworkCredential(emailUserName, emailPassward);
		//		emailClient.UseDefaultCredentials = false;
		//		//  emailClient.Timeout = 0;
		//		emailClient.Credentials = SMTPUserInfo;
		//		//Send Msg using smtp
		//		emailClient.Send(message);
		//	}
		//	catch (Exception ex)
		//	{
		//		_logger.LogError(ex.ToString());
		//	}
		//}

		//public void SendMail(string toEmail, string subject, string htmlBody, List<MailAttachmentDto>? attachments = null)
		//{
		//	using (var message = new MailMessage())
		//	{
		//		message.To.Add(toEmail);
		//		message.Subject = subject;
		//		message.Body = htmlBody;
		//		message.IsBodyHtml = true;
		//		message.From = new MailAddress(_config["EmailSettings:FromEmail"]);

		//		if (attachments != null && attachments.Count() > 0)
		//		{
		//			foreach (var a in attachments)
		//			{
		//				var stream = new MemoryStream(a.FileBytes);
		//				var attachment = new Attachment(stream, a.FileName, a.ContentType);
		//				message.Attachments.Add(attachment);
		//			}
		//		}

		//		using (var smtp = new SmtpClient(
		//			_config["EmailSettings:EmailHostName"],
		//			int.Parse(_config["EmailSettings:EmailPort"])
		//		))
		//		{
		//			smtp.Credentials = new NetworkCredential(
		//				_config["EmailSettings:EmailUserName"],
		//				_config["EmailSettings:EmailPassward"]
		//			);
		//			smtp.EnableSsl = true;
		//			smtp.Send(message);
		//		}
		//	}
		//}

		#region otp mails
		public void SendEmailOnForgotPassword(string email, string htmlBody)
		{
			try
			{
				email = email.Replace(" ", "+");
				if (rgEmail.IsMatch(email) && !string.IsNullOrEmpty(htmlBody))
				{
					string emailSubject = "Forgot Password";
					Email objEmail = new Email(_config, _logger);
					objEmail.SendMail(email, emailSubject, htmlBody);
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "email: " + email + " htmlBody: " + htmlBody);
			}
		}

		public void SendOtpMail(string email, string htmlBody)
		{
			try
			{
				email = email.Replace(" ", "+");
				if (rgEmail.IsMatch(email) && !string.IsNullOrEmpty(htmlBody))
				{
					string emailSubject = "Account verification";
					Email objEmail = new Email(_config, _logger);
					objEmail.SendMail(email, emailSubject, htmlBody);
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "email: " + email + " htmlBody: " + htmlBody);
			}
		}

		public void SendAccessCodeMail(string email, string htmlBody)
		{
			try
			{
				email = email.Replace(" ", "+");
				if (rgEmail.IsMatch(email) && !string.IsNullOrEmpty(htmlBody))
				{
					string emailSubject = "Sentinel App Access code";
					Email objEmail = new Email(_config, _logger);
					objEmail.SendMail(email, emailSubject, htmlBody);
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "email: " + email + " htmlBody: " + htmlBody);
			}
		}

		public void SendRequestAccessCodeMail(string email, string htmlBody)
		{
			try
			{
				email = email.Replace(" ", "+");
				if (rgEmail.IsMatch(email) && !string.IsNullOrEmpty(htmlBody))
				{
					string emailSubject = "Approval Request";
					Email objEmail = new Email(_config, _logger);
					objEmail.SendMail(email, emailSubject, htmlBody);
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "email: " + email + " htmlBody: " + htmlBody);
			}
		}
		public void SendRejectAccessCodeMail(string email, string htmlBody)
		{
			try
			{
				email = email.Replace(" ", "+");
				if (rgEmail.IsMatch(email) && !string.IsNullOrEmpty(htmlBody))
				{
					string emailSubject = "Access Request Rejected";
					Email objEmail = new Email(_config, _logger);
					objEmail.SendMail(email, emailSubject, htmlBody);
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "email: " + email + " htmlBody: " + htmlBody);
			}
		}
		public void SendEmailOnPdfExport(string email, string htmlBody, List<MailAttachmentDto>? attachments)
		{
			try
			{
				email = email.Replace(" ", "+");
				if (rgEmail.IsMatch(email) && !string.IsNullOrEmpty(htmlBody))
				{
					string emailSubject = "Sentinel App Medical History Report";
					Email objEmail = new Email(_config, _logger);
					objEmail.SendMail(email, emailSubject, htmlBody, attachments);
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "email: " + email + " htmlBody: " + htmlBody);
			}
		}
		public void SendEmptyEmailOnPdfExport(string email, string htmlBody)
		{
			try
			{
				email = email.Replace(" ", "+");
				if (rgEmail.IsMatch(email) && !string.IsNullOrEmpty(htmlBody))
				{
					string emailSubject = "Sentinel App Medical History Report";
					Email objEmail = new Email(_config, _logger);
					objEmail.SendMail(email, emailSubject, htmlBody);
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "email: " + email + " htmlBody: " + htmlBody);
			}
		}



		//public void SendOtpMail(string userName, string otp)
		//{
		//	Dictionary<string, string> messageText = new Dictionary<string, string>();
		//	var currDir = Directory.GetCurrentDirectory();
		//	var htmlFilePath = Path.Combine(currDir, "Html", "OtpMail.html");

		//	messageText.Add("{otp}", otp);
		//	messageText.Add("{Year}", DateTime.Now.Year.ToString());

		//	string emailSubject = "Account verification";

		//	Email objEmail = new Email(_config, _logger);
		//	try
		//	{
		//		objEmail.SendEmail(userName, emailSubject, messageText, htmlFilePath);
		//	}
		//	catch (Exception ex)
		//	{
		//		_logger.LogError(ex.ToString());
		//	}
		//}
		#endregion

	}
}
