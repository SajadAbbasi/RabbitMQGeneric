using System.Threading.Tasks;

namespace RabbitMQGeneric
{
    public interface IDomainEventHandler<TIDomainEvent> : IDomainEventHandler where TIDomainEvent : DomainEvent
    {
        Task Handle(TIDomainEvent @event);
    }

    public interface IDomainEventHandler
    {
    }
}
