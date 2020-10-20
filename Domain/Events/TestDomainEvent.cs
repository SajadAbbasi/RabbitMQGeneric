using RabbitMQGeneric;

namespace Domain.Events
{
    public class TestDomainEvent : DomainEvent
    {
        public string Message { get; set; }

        public TestDomainEvent(string messsge)
        {
            Message = messsge;
        }
    }
}
