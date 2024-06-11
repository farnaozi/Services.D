namespace Services.D.Core.Models
{
    public class AppSettings
    {
        public string? ConnectionString { get; set; }
        public string? RabbitMQHostName { get; set; }
        public string? UniqueId { get; set; }
    }
}
