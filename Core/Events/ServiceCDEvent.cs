using Services.D.Core.Models;

namespace Services.D.Core.Events
{
    public class ServiceCDEvent : Event
    {
        public string Message { get; set; }
    }
}
