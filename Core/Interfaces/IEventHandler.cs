using Services.D.Core.Models;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Services.D.Core.Interfaces
{
    public interface IEventHandler<in TEvent> : IEventHandler
         where TEvent : Event
    {
        Task Handle(TEvent @event);
    }

    public interface IEventHandler
    {

    }
}
