using Domain.Events;
using Microsoft.Extensions.Logging;
using RabbitMQGeneric;
using System.Threading.Tasks;

namespace Consumer.EventHandlers
{
    public class TestDomainEventHandler : IDomainEventHandler<TestDomainEvent>
    {
        private readonly ILogger _logger;
        public TestDomainEventHandler(ILogger<TestDomainEventHandler> logger)
        {
            _logger = logger;
        }

        public async Task Handle(TestDomainEvent @event)
        {
            _logger.LogInformation(@event.AggregateId.ToString());
            await Task.CompletedTask;
        }
    }
}
