using System;

namespace RabbitMQGeneric
{
    public class DomainEvent
    {
        public DomainEvent()
        {
            AggregateId = Guid.NewGuid();
            CreationDateTime = DateTime.Now;
        }

        public DomainEvent(Guid aggregateId, DateTime creationDateTime)
        {
            AggregateId = aggregateId;
            CreationDateTime = creationDateTime;
        }

        public Guid AggregateId { get; private set; }

        public DateTime CreationDateTime { get; private set; }
    }
}
