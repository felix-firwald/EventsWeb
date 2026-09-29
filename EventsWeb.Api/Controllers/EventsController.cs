using EventsWeb.Core.DTO;
using EventsWeb.Core.Entities;
using EventsWeb.Core.Interface;
using Microsoft.AspNetCore.Mvc;

namespace EventsWeb.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public sealed class EventsController : ControllerBase
    {
        private IEventService eventService { get; }
        public EventsController(IEventService service)
        {
            this.eventService = service;
        }
        [HttpGet]
        public ActionResult<PaginatedResult<Event>> GetAll(
            [FromQuery] string? title = null,
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            return this.Ok(this.eventService.GetEvents(new(page, pageSize), title, from, to));
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
            this.CheckDates(dto);
            if (!this.ModelState.IsValid)
            {
                return this.ValidationProblem(this.ModelState);
            }
            this.eventService.Create(dto);
            return this.Created();
        }

        [HttpPut("{id:guid}")]
        public IActionResult Put(Guid id, [FromBody] EventDto dto)
        {
            this.CheckDates(dto);
            if (!this.ModelState.IsValid)
            {
                return this.ValidationProblem(this.ModelState);
            }
            Event? found = this.eventService.GetById(id);
            if (found is null)
            {
                return this.NotFound();
            }           
            this.eventService.Update(found, dto);
            return this.NoContent();
        }

        private void CheckDates(EventDto dto)
        {
            if (dto.StartAt >= dto.EndAt)
            {
                this.ModelState.AddModelError(nameof(dto.EndAt), "EndAt must be later than StartAt.");
            }
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
