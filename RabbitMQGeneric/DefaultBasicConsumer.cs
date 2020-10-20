using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQGeneric
{
    public class DefaultBasicConsumer : AsyncDefaultBasicConsumer
    {
        private readonly IModel _channel;
        private readonly IServiceProvider _serviceProvider;

        public Dictionary<string, (Type @event, List<Type> eventHandlers)> _handlers = new Dictionary<string, (Type, List<Type>)>();

        public DefaultBasicConsumer(IModel channel, IServiceProvider serviceProvider, Dictionary<string, (Type @event, List<Type> eventHandlers)> handlers)
        {
            _channel = channel;
            _serviceProvider = serviceProvider;
            _handlers = handlers;
        }

        public override async Task HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, ReadOnlyMemory<byte> body)
        {
            var message = Encoding.UTF8.GetString(body.ToArray());

            if (_handlers.ContainsKey(routingKey))
            {
                using var scope = _serviceProvider.CreateScope();
                foreach (var subscription in _handlers[routingKey].eventHandlers)
                {
                    var handler = scope.ServiceProvider.GetService(subscription);
                    if (handler == null) continue;
                    var eventType = _handlers[routingKey].@event;
                    var domainEvent = JsonConvert.DeserializeObject(message, eventType);
                    var eventHandler = typeof(IDomainEventHandler<>).MakeGenericType(eventType);
                    await (Task)eventHandler.GetMethod("Handle").Invoke(handler, new object[] { domainEvent });
                }
            }
            _channel.BasicAck(deliveryTag, false);
        }
    }
}
