namespace Services.D.Core.Dtos
{
    public class ServiceResponseBase
    {
        public bool Success { get; set; }
        public string ExceptionCode { get; set; }
        public string ExceptionMessage { get; set; }
    }
}