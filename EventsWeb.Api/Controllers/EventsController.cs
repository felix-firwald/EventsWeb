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
        private IBookingService bookingService { get; }
        public EventsController(IEventService service, IBookingService bookingService)
        {
            this.eventService = service;
            this.bookingService = bookingService;
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Get(Guid id)
        {
            Event found = this.eventService.GetById(id);
            return this.Ok(found);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        [HttpPost("{id:guid}/book")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Booking>> CreateBooking([FromRoute] Guid id)
        {
            Booking b = await this.bookingService.CreateBookingAsync(id);
            return this.Accepted($"/bookings/{b.Id}", b);
        }

        private void CheckDates(EventDto dto)
        {
            if (dto.StartAt >= dto.EndAt)
            {
                this.ModelState.AddModelError(nameof(dto.EndAt), "EndAt must be later than StartAt.");
            }
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(Guid id)
        {
            Event found = this.eventService.GetById(id);
            this.eventService.Delete(found);
            return this.NoContent();
        }
    }
}
