using EventsWeb.Core.Entities;
using EventsWeb.Core.Interface;
using Microsoft.AspNetCore.Mvc;

namespace EventsWeb.Api.Controllers
{
    [Route("bookings")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private IBookingService bookingService { get; }
        public BookingController(IBookingService bookingService)
        {
            this.bookingService = bookingService;
        }

        [HttpGet("{bookingId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Booking>> GetBookingByIdAsync(Guid bookingId)
        {
            Booking booking = await this.bookingService.GetBookingByIdAsync(bookingId);
            return this.Ok(booking);
        }
    }
}
