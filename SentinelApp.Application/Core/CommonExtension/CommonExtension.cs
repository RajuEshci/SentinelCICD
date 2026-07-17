using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SentinelApp.Application.Core.CommonExtension
{
	public class CommonExtension
	{
		public static string GetImageExtensionFromBytes(byte[] fileBytes)
		{
			if (fileBytes == null || fileBytes.Length < 4)
				return ".bin";
			if (fileBytes[0] == 0xFF && fileBytes[1] == 0xD8)
				return ".jpg";
			if (fileBytes[0] == 0x89 && fileBytes[1] == 0x50 && fileBytes[2] == 0x4E && fileBytes[3] == 0x47)
				return ".png";
			if (fileBytes[0] == 0x47 && fileBytes[1] == 0x49 && fileBytes[2] == 0x46)
				return ".gif";
			if (fileBytes[0] == 0x42 && fileBytes[1] == 0x4D)
				return ".bmp";
			if (fileBytes[0] == 0x52 && fileBytes[1] == 0x49 && fileBytes[2] == 0x46 && fileBytes[3] == 0x46)
				return ".webp";
			if (fileBytes[0] == 0x25 && fileBytes[1] == 0x50 && fileBytes[2] == 0x44 && fileBytes[3] == 0x46)
				return ".pdf";
			if (fileBytes[0] == 0x50 && fileBytes[1] == 0x4B)
			{
				return ".zip";
			}
			if (fileBytes[0] == 0xD0 && fileBytes[1] == 0xCF && fileBytes[2] == 0x11 && fileBytes[3] == 0xE0)
				return ".xls";

			if (fileBytes[0] == 0x50 && fileBytes[1] == 0x4B)
			{
				string content = Encoding.UTF8.GetString(fileBytes);
				if (content.Contains("xlsx") || content.Contains("xl/"))
					return ".xlsx";

				return ".zip";
			}
			if (fileBytes[0] == 0xD0 && fileBytes[1] == 0xCF && fileBytes[2] == 0x11 && fileBytes[3] == 0xE0)
				return ".doc";
			if (fileBytes[0] == 0x7B && fileBytes[1] == 0x5C && fileBytes[2] == 0x72 && fileBytes[3] == 0x74)
				return ".rtf";
			if (fileBytes[0] < 128)
			{
				string text = Encoding.UTF8.GetString(fileBytes);

				if (text.Contains(","))
					return ".csv";

				return ".txt";
			}
			if (fileBytes[0] < 128)
				return ".txt";
			if (fileBytes[0] == 0x49 && fileBytes[1] == 0x44 && fileBytes[2] == 0x33)
				return ".mp3";
			if (fileBytes[0] == 0x52 && fileBytes[1] == 0x49 && fileBytes[2] == 0x46 && fileBytes[3] == 0x46)
				return ".wav";
			if (fileBytes[4] == 0x66 && fileBytes[5] == 0x74 && fileBytes[6] == 0x79 && fileBytes[7] == 0x70)
				return ".mp4";

			if (fileBytes[0] == 0x52 && fileBytes[1] == 0x49 && fileBytes[2] == 0x46 && fileBytes[3] == 0x46)
				return ".avi";

			if (fileBytes[0] == 0x50 && fileBytes[1] == 0x4B)
				return ".zip";

			if (fileBytes[0] == 0x52 && fileBytes[1] == 0x61 && fileBytes[2] == 0x72 && fileBytes[3] == 0x21)
				return ".rar";

			if (fileBytes[0] == 0x37 && fileBytes[1] == 0x7A && fileBytes[2] == 0xBC)
				return ".7z";

			return ".bin";
		}

		public static KeyValuePair<string, string> GetContentType(string extension)
		{
			if (string.IsNullOrWhiteSpace(extension))
				return new KeyValuePair<string, string>("", "");

			extension = extension.Replace(".", "").ToLower();

			switch (extension)
			{
				case "jpg":
				case "jpeg":
					return new KeyValuePair<string, string>(extension, "image/jpeg");

				case "png":
					return new KeyValuePair<string, string>(extension, "image/png");

				case "gif":
					return new KeyValuePair<string, string>(extension, "image/gif");

				case "bmp":
					return new KeyValuePair<string, string>(extension, "image/bmp");

				case "webp":
					return new KeyValuePair<string, string>(extension, "image/webp");

				case "pdf":
					return new KeyValuePair<string, string>(extension, "application/pdf");

				case "doc":
					return new KeyValuePair<string, string>(extension, "application/msword");

				case "docx":
					return new KeyValuePair<string, string>(extension,
						"application/vnd.openxmlformats-officedocument.wordprocessingml.document");

				case "xls":
					return new KeyValuePair<string, string>(extension, "application/vnd.ms-excel");

				case "xlsx":
					return new KeyValuePair<string, string>(extension,
						"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

				case "ppt":
					return new KeyValuePair<string, string>(extension, "application/vnd.ms-powerpoint");

				case "pptx":
					return new KeyValuePair<string, string>(extension,
						"application/vnd.openxmlformats-officedocument.presentationml.presentation");

				case "txt":
					return new KeyValuePair<string, string>(extension, "text/plain");

				case "rtf":
					return new KeyValuePair<string, string>(extension, "application/rtf");

				case "mp3":
					return new KeyValuePair<string, string>(extension, "audio/mpeg");

				case "wav":
					return new KeyValuePair<string, string>(extension, "audio/wav");

				case "mp4":
					return new KeyValuePair<string, string>(extension, "video/mp4");

				case "avi":
					return new KeyValuePair<string, string>(extension, "video/x-msvideo");

				case "zip":
					return new KeyValuePair<string, string>(extension, "application/zip");

				case "rar":
					return new KeyValuePair<string, string>(extension, "application/vnd.rar");

				case "7z":
					return new KeyValuePair<string, string>(extension, "application/x-7z-compressed");

				case "csv":
					return new KeyValuePair<string, string>(extension, "text/csv");

				case "xlsm":
					return new KeyValuePair<string, string>(extension, "vnd.ms-excel.sheet.macroenabled.12");

				default:
					return new KeyValuePair<string, string>(extension, "application/octet-stream");
			}
		}

		public static string GetFileExtensionFromDataUri(string dataUri)
		{
			if (string.IsNullOrWhiteSpace(dataUri))
				return ".bin";

			try
			{
				var prefixEnd = dataUri.IndexOf(';');
				if (prefixEnd < 0)
					return ".bin";

				string typePart = dataUri.Substring(0, prefixEnd);
				string[] parts = typePart.Split('/');
				if (parts.Length < 2)
					return ".bin";

				string fileType = parts[1].ToLower();

				return fileType switch
				{
					"csv" => ".csv",
					"txt" => ".txt",
					"pdf" => ".pdf",
					"vnd.ms-excel" => ".xls",
					"vnd.openxmlformats-officedocument.spreadsheetml.sheet" => ".xlsx",
					"doc" => ".doc",
					"docx" => ".docx",
					"ppt" => ".ppt",
					"pptx" => ".pptx",
					"jpg" => ".jpg",
					"jpeg" => ".jpg",
					"png" => ".png",
					"gif" => ".gif",
					"bmp" => ".bmp",
					"vnd.ms-excel.sheet.macroenabled.12" => ".xlsm",
					_ => ".bin"
				};
			}
			catch
			{
				return ".bin";
			}
		}
		public static string GetUniqueNumber(int maxSize)
		{
			char[] chars = new char[62];
			//chars = "1234567890".ToCharArray();
			chars = "1234567890ABCDEFGHJKLMOPQRSTUVWXYZ".ToCharArray();
			byte[] data = new byte[1];
			RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();

			crypto.GetNonZeroBytes(data);
			data = new byte[maxSize];
			crypto.GetNonZeroBytes(data);

			StringBuilder result = new StringBuilder(maxSize);
			foreach (byte b in data)
			{
				result.Append(chars[b % (chars.Length)]);
			}
			return result.ToString();
		}

		public static string GetUniqueKey(int maxSize)
		{
			char[] chars = new char[62];
			chars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
			byte[] data = new byte[1];
			RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
			crypto.GetNonZeroBytes(data);
			data = new byte[maxSize];
			crypto.GetNonZeroBytes(data);

			StringBuilder result = new StringBuilder(maxSize);
			foreach (byte b in data)
			{
				result.Append(chars[b % (chars.Length)]);
			}
			return result.ToString();
		}
		public static string GeneratePassword(int length, bool isUpperCase, bool isLowerCase, bool isNumber, bool isSpecialChar)
		{
			const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
			const string lower = "abcdefghijklmnopqrstuvwxyz";
			const string numbers = "0123456789";
			const string special = "!@#$%^&*()_+-=[]{};':\"\\|,.<>/?";

			var requiredChars = new List<char>();
			var allChars = new StringBuilder();

			using (var crypto = new RNGCryptoServiceProvider())
			{
				if (isUpperCase)
				{
					requiredChars.Add(upper[GetRandomIndex(crypto, upper.Length)]);
					allChars.Append(upper);
				}

				if (isLowerCase)
				{
					requiredChars.Add(lower[GetRandomIndex(crypto, lower.Length)]);
					allChars.Append(lower);
				}

				if (isNumber)
				{
					requiredChars.Add(numbers[GetRandomIndex(crypto, numbers.Length)]);
					allChars.Append(numbers);
				}

				if (isSpecialChar)
				{
					requiredChars.Add(special[GetRandomIndex(crypto, special.Length)]);
					allChars.Append(special);
				}

				if (allChars.Length == 0)
					throw new ArgumentException("At least one character type must be selected.");

				if (length < requiredChars.Count)
					throw new ArgumentException($"Password length must be at least {requiredChars.Count}.");

				while (requiredChars.Count < length)
				{
					requiredChars.Add(allChars[GetRandomIndex(crypto, allChars.Length)]);
				}

				// Shuffle
				for (int i = requiredChars.Count - 1; i > 0; i--)
				{
					int j = GetRandomIndex(crypto, i + 1);
					(requiredChars[i], requiredChars[j]) = (requiredChars[j], requiredChars[i]);
				}

				return new string(requiredChars.ToArray());
			}
		}

		private static int GetRandomIndex(RNGCryptoServiceProvider crypto, int maxValue)
		{
			byte[] data = new byte[4];
			crypto.GetBytes(data);
			return Math.Abs(BitConverter.ToInt32(data, 0)) % maxValue;
		}
	}
}
