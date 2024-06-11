using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;
using Services.D.Core.Models;
using Services.D.Core.Enums;

namespace Services.D.Core.Interfaces
{
    public interface IEventBus
    {
        void Publish<T>(T @event, ExchangeTypes exchangeType, bool createQueue = true) where T : Event;

        void Subscribe<T, TH>(ExchangeTypes exchangeType)
            where T : Event
            where TH : IEventHandler<T>;
    }
}
