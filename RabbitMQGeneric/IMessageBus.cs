namespace RabbitMQGeneric
{
    public interface IMessageBus
    {
        void Publish(DomainEvent @event, string exchangeName = null);

        void Subscribe<TDomainEvent, TDomainEventHandle>(string exchangeName = null, string queueName = null)
            where TDomainEvent : DomainEvent
            where TDomainEventHandle : IDomainEventHandler<TDomainEvent>;
    }
}
