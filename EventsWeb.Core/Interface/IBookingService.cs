using EventsWeb.Core.Entities;
using EventsWeb.Core.Exceptions;

namespace EventsWeb.Core.Interface
{
    /// <summary>
    /// Represents service for working with bookings
    /// </summary>
    public interface IBookingService
    {
        /// <summary>
        /// Confirms booking
        /// </summary>
        public Task Confirm(Guid bookingId);

        /// <summary>
        /// Rejects booking
        /// </summary>
        public Task Reject(Guid bookingId);

        /// <summary>
        /// Gets pending bookings
        /// </summary>
        public Task<IEnumerable<Booking>> GetPending();

        /// <summary>
        /// Creates new <see cref="Booking"/> for the specified <see cref="Event"/> if last exists, otherwise throws a <see cref="NotFoundException"/> 
        /// </summary>
        public Task<Booking> CreateBookingAsync(Guid eventId);

        /// <summary>
        /// Get <see cref="Booking"/> by its ID if exists, otherwise throws a <see cref="NotFoundException"/>
        /// </summary>
        public Task<Booking> GetBookingByIdAsync(Guid bookingId);
    }
}
