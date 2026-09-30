using EventsWeb.Core.Enums;

namespace EventsWeb.Core.Entities
{
    /// <summary>
    /// Represents a booking record for the specified event
    /// </summary>
    public sealed class Booking
    {
        /// <summary>
        /// Represents ID of entity
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Represents ID of <see cref="Event"/>
        /// </summary>
        public Guid EventId { get; set; }

        /// <summary>
        /// Represents current status of booking
        /// </summary>
        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        /// <summary>
        /// Represents the date and time when the record was created
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Represents the date and time when the booking was processed by the system 
        /// and received one of the statuses (<see cref="BookingStatus.Confirmed"/> or <see cref="BookingStatus.Rejected"/>)
        /// </summary>
        public DateTime? LastUpdatedAt { get; set; }
    }
}
