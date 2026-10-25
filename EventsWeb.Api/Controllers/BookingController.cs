using EventsWeb.Core.Entities;
using EventsWeb.Core.Interface;
using Microsoft.AspNetCore.Mvc;

namespace EventsWeb.Api.Controllers
{
    [Route("api/bookings")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private IBookingService bookingService { get; }
        public BookingController(IBookingService bookingService)
        {
            this.bookingService = bookingService;
        }

        [HttpGet("{bookingId:guid}")]
        public async Task<ActionResult<Booking>> GetBookingByIdAsync(Guid bookingId)
        {
            Booking booking = await this.bookingService.GetBookingByIdAsync(bookingId);
            return this.Ok(booking);
        }
    }
}
