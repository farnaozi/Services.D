using Services.D.Core.Enums;
using Services.D.Core.Interfaces;
using Services.D.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Services.D.Infrastructure
{
    public sealed class RabbitMQBus : IEventBus
    {
        private readonly Dictionary<string, List<Type>> _handlers;
        private readonly List<Type> _eventTypes;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly AppSettings _appSettings;
        private readonly Guid uniqueId;

        public RabbitMQBus(IServiceScopeFactory serviceScopeFactory, IOptions<AppSettings> appSettings)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _handlers = new Dictionary<string, List<Type>>();
            _eventTypes = new List<Type>();
            _appSettings = appSettings.Value;
            uniqueId = Guid.Parse(_appSettings.UniqueId!);
        }

        public void Publish<T>(T @event, ExchangeTypes exchangeType, bool createQueue = true) where T : Event
        {
            var factory = new ConnectionFactory() { HostName = _appSettings.RabbitMQHostName };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                var eventName = @event.GetType().Name;
                var exchangeTpeString = exchangeType.ToString().ToLower();

                channel.ExchangeDeclare(eventName, exchangeTpeString);
                if(createQueue)
                {
                    channel.QueueDeclare(exchangeType == ExchangeTypes.Direct
                        ? eventName : $"{uniqueId}-{eventName}", true, false, false, null);
                    channel.QueueBind(exchangeType == ExchangeTypes.Direct
                        ? eventName : $"{uniqueId}-{eventName}", eventName, eventName, null);
                }
                var message = JsonConvert.SerializeObject(@event);
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(eventName, eventName, null, body);
            }
        }

        public void Subscribe<T, TH>(ExchangeTypes exchangeType)
            where T : Event
            where TH : IEventHandler<T>
        {
            var eventName = typeof(T).Name;
            var handlerType = typeof(TH);

            if (!_eventTypes.Contains(typeof(T)))
            {
                _eventTypes.Add(typeof(T));
            }

            if (!_handlers.ContainsKey(eventName))
            {
                _handlers.Add(eventName, new List<Type>());
            }

            if (_handlers[eventName].Any(s => s.GetType() == handlerType))
            {
                throw new ArgumentException(
                    $"Handler Type {handlerType.Name} already is registered for '{eventName}'", nameof(handlerType));
            }

            _handlers[eventName].Add(handlerType);

            StartBasicConsume<T>(exchangeType);
        }

        private void StartBasicConsume<T>(ExchangeTypes exchangeType) where T : Event
        {
            var factory = new ConnectionFactory()
            {
                HostName = _appSettings.RabbitMQHostName,
                DispatchConsumersAsync = true
            };

            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            var eventName = typeof(T).Name;
            var exchangeTpeString = exchangeType.ToString().ToLower();

            channel.ExchangeDeclare(eventName, exchangeTpeString);
            channel.QueueDeclare(exchangeType == ExchangeTypes.Direct
                ? eventName : $"{uniqueId}-{eventName}", true, false, false, null);
            channel.QueueBind(exchangeType == ExchangeTypes.Direct
                ? eventName : $"{uniqueId}-{eventName}", eventName, eventName, null);
            channel.BasicQos(0, 1, false);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.Received += Consumer_Received;

            channel.BasicConsume(exchangeType == ExchangeTypes.Direct
                ? eventName : $"{uniqueId}-{eventName}", true, consumer);
        }

        private async Task Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            var eventName = e.RoutingKey;
            var message = Encoding.UTF8.GetString(e.Body.ToArray());

            try
            {
                await ProcessEvent(eventName, message).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
            }
        }

        private async Task ProcessEvent(string eventName, string message)
        {
            if (_handlers.ContainsKey(eventName))
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var subscriptions = _handlers[eventName];
                    foreach (var subscription in subscriptions)
                    {
                        var handler = scope.ServiceProvider.GetService(subscription);
                        if (handler == null) continue;
                        var eventType = _eventTypes.SingleOrDefault(t => t.Name == eventName);
                        var @event = JsonConvert.DeserializeObject(message, eventType);
                        var conreteType = typeof(IEventHandler<>).MakeGenericType(eventType);
                        await (Task)conreteType.GetMethod("Handle").Invoke(handler, new object[] { @event });
                    }
                }
            }
        }
    }
}