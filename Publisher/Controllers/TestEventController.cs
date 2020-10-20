using Domain.Events;
using Microsoft.AspNetCore.Mvc;
using RabbitMQGeneric;

namespace Publisher.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestEventController : ControllerBase
    {
        private readonly IEventBus _eventBus;

        public TestEventController(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        [HttpPost]
        public ActionResult Send()
        {
            var ev = new TestDomainEvent("Test");
            _eventBus.Publish(ev);
            return Accepted();
        }
    }
}
