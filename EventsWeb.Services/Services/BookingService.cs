using EventsWeb.Core.Entities;
using EventsWeb.Core.Enums;
using EventsWeb.Core.Exceptions;
using EventsWeb.Core.Interface;

namespace EventsWeb.Services.Services
{
    public sealed class BookingService : IBookingService
    {
        private List<Booking> bookings { get; } = [];
        private object locker { get; } = new();
        private IEventService eventService { get; }
        public BookingService(IEventService eventService)
        {
            this.eventService = eventService;
        }
        public async Task<Booking> CreateBookingAsync(Guid eventId)
        {
            Booking booking;
            lock (this.locker)
            {
                Event result = this.eventService.GetById(eventId);
                booking = new()
                {
                    Id = Guid.NewGuid(),
                    EventId = eventId,
                    CreatedAt = DateTime.UtcNow
                };
                if (result.TryReserveSeats())
                {
                    this.bookings.Add(booking);
                }
                else
                {
                    throw new NoAvailableSeatsException("No available seats for this event");
                }
            }
            return booking;
        }

        public async Task<Booking> GetBookingByIdAsync(Guid bookingId)
        {
            Booking? result;
            lock (this.locker)
            {
                result = this.bookings.SingleOrDefault(b => b.Id == bookingId);
            }
            if (result == null)
            {
                throw new NotFoundException();
            }
            return result;
        }

        public async Task<IEnumerable<Booking>> GetPending()
        {   
            lock (this.locker)
            {
                return this.bookings.Where(b => b.Status == BookingStatus.Pending).ToArray();
            }            
        }

        public async Task Confirm(Guid bookingId)
        {
            Booking result = await this.GetBookingByIdAsync(bookingId);     
            lock (this.locker)
            {
                result.Confirm();
            }
        }

        public async Task Reject(Guid bookingId)
        {
            Booking result = await this.GetBookingByIdAsync(bookingId);           
            lock (this.locker)
            {
                result.Reject();
            }
        }
    }
}
