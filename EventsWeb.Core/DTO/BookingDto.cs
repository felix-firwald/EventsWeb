using EventsWeb.Core.Entities;
using EventsWeb.Core.Enums;

namespace EventsWeb.Core.DTO
{
    /// <summary>
    /// Represents DTO for <see cref="Booking"/>
    /// </summary>
    public sealed class BookingDto
    {
        /// <summary>
        /// Represents ID of entity
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Represents ID of <see cref="Event"/>
        /// </summary>
        public Guid EventId { get; }

        /// <summary>
        /// Represents current status of booking
        /// </summary>
        public BookingStatus Status { get; }

        /// <summary>
        /// Represents the date and time when the record was created
        /// </summary>
        public DateTime CreatedAt { get; }

        /// <summary>
        /// Represents the date and time when the booking was processed by the system 
        /// and received one of the statuses (<see cref="BookingStatus.Confirmed"/> or <see cref="BookingStatus.Rejected"/>)
        /// </summary>
        public DateTime? ProcessedAt { get; }

        /// <summary>
        /// Single constructor
        /// <para/> we can create it only from <see cref="Booking"/> instance
        /// </summary>
        public BookingDto(Booking input)
        {
            this.Id = input.Id;
            this.EventId = input.EventId;
            this.Status = input.Status;
            this.CreatedAt = input.CreatedAt;
            this.ProcessedAt = input.ProcessedAt;
        }

        /// <summary>
        /// Semantic sugar :)
        /// </summary>
        public static implicit operator BookingDto(Booking b)
        {
            return new(b);
        }
    }
}
