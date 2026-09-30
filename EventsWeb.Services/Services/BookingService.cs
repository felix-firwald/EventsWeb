using EventsWeb.Core.Entities;
using EventsWeb.Core.Interface;

namespace EventsWeb.Services.Services
{
    public sealed class BookingService : IBookingService
    {
        public BookingService()
        {

        }
        public async Task CreateBookingAsync(Guid eventId)
        {
            throw new NotImplementedException();
        }

        public async Task<Booking> GetBookingByIdAsync(Guid bookingId)
        {
            throw new NotImplementedException();
        }
    }
}
