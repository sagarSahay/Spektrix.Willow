using Microsoft.AspNetCore.Mvc;

namespace Willow.API.Events
{
    using System;
    using System.Threading.Tasks;
    using Commands;
    using ControllerData;
    using MassTransit;

    [Route("api/events")]
    public class EventController : Controller
    {
        private readonly ISendEndpoint sendEndpoint;

        public EventController(ISendEndpoint sendEndpoint)
        {
            this.sendEndpoint = sendEndpoint ?? throw new ArgumentNullException(nameof(sendEndpoint));
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> CreateEvent([FromBody] EventData input)
        {
            var id = Guid.NewGuid();
            var cmd = new CreateEvent()
            {
                Title = input.Title,
                Description = input.Description,
                EventId = id
            };

            await sendEndpoint.Send(cmd);

            return Accepted(new EventCreateResponse {EventId = id});
        }

        [HttpPatch]
        [Route("{id}/title/{newTitle}")]
        public async Task<IActionResult> ChangeTitle(Guid id, string newTitle)
        {
            if (string.IsNullOrEmpty(newTitle))
            {
                return BadRequest("Title cannot be blank");
            }

            if (id == Guid.Empty)
            {
                return BadRequest("Please provide event id");
            }

            var cmd = new UpdateEventTitle
            {
                EventId = id,
                Title = newTitle
            };
            await sendEndpoint.Send(cmd);

            return Ok();
        }
    }
}