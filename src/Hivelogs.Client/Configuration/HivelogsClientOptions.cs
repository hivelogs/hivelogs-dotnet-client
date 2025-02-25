namespace Hivelogs.Client.Configuration
{
    public class HivelogsClientOptions
    {
        public string ApiUrl { get; set; } = string.Empty;
        public Guid ApplicationEnvironmentId { get; set; }
        public bool UseBatchLogging { get; set; } = false;
    }
}
