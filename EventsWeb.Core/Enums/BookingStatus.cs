using EventsWeb.Core.Entities;

namespace EventsWeb.Core.Enums
{
    /// <summary>
    /// Represents status of <see cref="Booking"/>
    /// </summary>
    public enum BookingStatus : byte
    {
        /// <summary>
        /// The booking has been created and is awaiting processing
        /// </summary>
        Pending = 1,

        /// <summary>
        /// The booking has been confirmed
        /// </summary>
        Confirmed = 2,

        /// <summary>
        /// The booking has been declined
        /// </summary>
        Rejected = 3
    }
}
