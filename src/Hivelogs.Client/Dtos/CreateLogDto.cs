namespace Hivelogs.Client.Dtos
{
    public class CreateLogDto
    {
        public Guid ApplicationEnvironmentId { get; set; }
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
        public string Level { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? Exception { get; set; }
        public string CorrelationId { get; set; } = string.Empty;
        public string? RequestPath { get; set; }
        public string? HttpMethod { get; set; }
        public string? RequestBody { get; set; }
        public string? ResponseBody { get; set; }
        public string? ResponseStatus { get; set; }
    }
}
