namespace SentinelApp.Domain.Entities
{
    public class AppVersionEntity
    {
        public int Id { get; set; }
        public string VersionNo { get; set; }
        public bool IsForcefullyUpdate { get; set; }
        public string Message { get; set; }
    }

    public class VersionMessageEntity
    {
        public int Id { get; set; }
        public int VersionId { get; set; }
        public int LanguageId { get; set; }
        public string Message { get; set; }
    }
}