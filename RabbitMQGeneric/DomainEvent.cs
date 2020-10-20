using System;

namespace RabbitMQGeneric
{
    public class DomainEvent
    {
        public DomainEvent()
        {
            AggregateId = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;
        }

        public DomainEvent(Guid aggregateId, DateTime createDate)
        {
            AggregateId = aggregateId;
            CreationDate = createDate;
        }

        public Guid AggregateId { get; private set; }

        public DateTime CreationDate { get; private set; }
    }
}
