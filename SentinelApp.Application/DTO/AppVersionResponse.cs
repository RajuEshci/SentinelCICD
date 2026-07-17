using System.Collections.Generic;

namespace SentinelApp.Application.DTO
{
    public class AppVersionResponse
    {
        public string LatestVersion { get; set; }
        public bool UpdateRequired { get; set; }
        public List<AppVersionMessages> Messages { get; set; } = new List<AppVersionMessages>();
    }

    public class AppVersionMessages
    {
        public string Messages { get; set; }
    }
}