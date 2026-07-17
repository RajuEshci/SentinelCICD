namespace SentinelApp.Application.DTO
{
	public class Ref_RequestData
	{
		public byte[] RequestData { get; set; }
		private string ReadableData = "";

		public string GetData() => ReadableData;
		public void SetData(string data) => ReadableData = data;
	}
}
