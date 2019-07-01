namespace Willow.Query.Event
{
    using System.Linq;
    using System.Threading.Tasks;
    using Denormalizer.Repositories.Event;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/events")]
    public class EventController: Controller
    {
        private readonly IEventQueryRepository repository;

        public EventController(IEventQueryRepository repository)
        {
            this.repository = repository;
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var eventObj = await repository.GetById(id);

            if (eventObj == null) return NotFound();

            return Ok(eventObj.VM);
        }
        
        [HttpGet("")]
        public async Task<IActionResult> GetAll()
        {
            var eventObjLs = await repository.LoadAll();

            if (eventObjLs == null) return NotFound();

            return Ok(eventObjLs);
        }
    }
}