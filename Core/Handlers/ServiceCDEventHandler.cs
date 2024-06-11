using Services.D.Core.Events;
using Services.D.Core.Interfaces;
using System.Threading.Tasks;

namespace Services.D.Core.Handlers
{
    public class ServiceCDEventHandler : IEventHandler<ServiceCDEvent>
    {
        private readonly IServiceRepo _serviceRepo;

        public ServiceCDEventHandler(IServiceRepo serviceRepo)
        {
            _serviceRepo = serviceRepo;
        }

        public Task Handle(ServiceCDEvent @event)
        {
            _serviceRepo.GetMessage(@event.Message);
            return Task.CompletedTask;
        }
    }
}
