using EventsWeb.Core.DTO;
using EventsWeb.Core.Entities;
using EventsWeb.Core.Interface;
using Microsoft.AspNetCore.Mvc;

namespace EventsWeb.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public sealed class EventsController : ControllerBase
    {
        private IEventService eventService { get; }
        public EventsController(IEventService service)
        {
            this.eventService = service;
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            return this.Ok(this.eventService.GetEvents());
        }

        [HttpGet("{id:guid}")]
        public IActionResult Get(Guid id)
        {
            Event? found = this.eventService.GetById(id);
            if (found is null)
            {
                return this.NotFound();
            }
            return this.Ok(found);
        }

        [HttpPost]
        public IActionResult Post([FromBody] EventDto dto)
        {
            this.eventService.Create(dto);
            return this.Created();
        }

        [HttpPut("{id:guid}")]
        public IActionResult Put(Guid id, [FromBody] EventDto dto)
        {
            Event? found = this.eventService.GetById(id);
            if (found is null)
            {
                return this.NotFound();
            }
            this.eventService.Update(found, dto);
            return this.NoContent();
        }

        [HttpDelete("{id:guid}")]
        public IActionResult Delete(Guid id)
        {
            Event? found = this.eventService.GetById(id);
            if (found is null)
            {
                return this.NotFound();
            }
            this.eventService.Delete(found);
            return this.NoContent();
        }
    }
}
