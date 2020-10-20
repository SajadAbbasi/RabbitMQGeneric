using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RabbitMQGeneric
{
    public class EventBus : IEventBus, IDisposable
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly IServiceProvider _serviceProvider;

        private IConnection _connection;
        private string _queueName;
        private string _exchangeName;

        private List<IModel> _consumerChannels = new List<IModel>();
        private Dictionary<string, (Type @event, List<Type> eventHandlers)> _handlers = new Dictionary<string, (Type, List<Type>)>();

        public EventBus(IConnectionFactory connectionFactory, IServiceProvider serviceProvider, string exchangeName = "event.bus", string queueName = "default")
        {
            _connectionFactory = connectionFactory;
            _serviceProvider = serviceProvider;
            _exchangeName = exchangeName;
            _queueName = queueName;
        }

        public void Publish(DomainEvent @event, string exchangeName = null)
        {
            CheckConnection();
            var eventName = @event.GetType().Name;
            var exName = exchangeName ?? _exchangeName;

            using var channel = _connection.CreateModel();
            channel.ExchangeDeclare(exchange: exName, type: "direct");

            var message = JsonConvert.SerializeObject(@event);
            var body = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(exchange: exName, routingKey: eventName, mandatory: true, body: body);
        }

        public void Subscribe<TDomainEvent, TDomainEventHandler>(string exchangeName = null, string queueName = null)
            where TDomainEvent : DomainEvent
            where TDomainEventHandler : IDomainEventHandler<TDomainEvent>
        {
            var exName = exchangeName ?? _exchangeName;
            var qName = queueName ?? _queueName;

            CheckConnection();

            var consumerChannel = _connection.CreateModel();
            consumerChannel.ExchangeDeclare(exchange: exName, type: "direct");
            consumerChannel.QueueDeclare(queue: qName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            var eventName = typeof(TDomainEvent).Name;
            if (!_handlers.ContainsKey(eventName))
            {
                using var channel = _connection.CreateModel();
                channel.QueueBind(queue: _queueName, exchange: exName, routingKey: eventName);
                _handlers.Add(eventName, (typeof(TDomainEvent), new List<Type>()));
            }

            _handlers[eventName].eventHandlers.Add(typeof(TDomainEventHandler));

            var consumer = new EventBasicConsumer(consumerChannel, _serviceProvider, _handlers);
            consumerChannel.BasicConsume(queue: qName, autoAck: false, consumer: consumer);
            _consumerChannels.Add(consumerChannel);
        }

        private void CheckConnection()
        {
            if (!(_connection != null && _connection.IsOpen))
                _connection = _connectionFactory.CreateConnection();
        }

        public void Dispose()
        {
            if (_consumerChannels.Any())
                _consumerChannels.ForEach(c => c.Dispose());
        }
    }
}
