using Domain.Events;
using Microsoft.AspNetCore.Mvc;
using RabbitMQGeneric;

namespace Publisher.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestEventController : ControllerBase
    {
        private readonly IMessageBus _messageBus;

        public TestEventController(IMessageBus messageBus)
        {
            _messageBus = messageBus;
        }

        [HttpPost]
        public ActionResult Send()
        {
            var ev = new TestDomainEvent("Test");
            _messageBus.Publish(ev);
            return Accepted();
        }
    }
}
