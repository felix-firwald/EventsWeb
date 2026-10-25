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
            Event result = this.eventService.GetById(eventId);
            Booking booking = new()
            {
                Id = Guid.NewGuid(),
                EventId = eventId,
                CreatedAt = DateTime.UtcNow
            };
            lock (this.locker)
            {
                this.bookings.Add(booking);
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
            if (result.Status != BookingStatus.Pending)
            {
                throw new InvalidOperationException("Booking is not pending.");
            }
            lock (this.locker)
            {
                result.Status = BookingStatus.Confirmed;
                result.ProcessedAt = DateTime.UtcNow;
            }
        }

        public async Task Reject(Guid bookingId)
        {
            Booking result = await this.GetBookingByIdAsync(bookingId);
            if (result.Status != BookingStatus.Pending)
            {
                throw new InvalidOperationException("Booking is not pending.");
            }
            lock (this.locker)
            {
                result.Status = BookingStatus.Rejected;
                result.ProcessedAt = DateTime.UtcNow;
            }
        }
    }
}
