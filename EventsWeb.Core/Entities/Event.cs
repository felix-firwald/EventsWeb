namespace EventsWeb.Core.Entities
{
    public class Event
    {
        /// <summary>
        /// Represents unique ID of event
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Represents
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Represents
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Represents start date and time of the event
        /// </summary>
        public DateTime StartAt { get; set; }

        /// <summary>
        /// Represents end date and time of the event
        /// </summary>
        public DateTime EndAt { get; set; }

        /// <summary>
        /// Represents total count of seats
        /// </summary>
        public required int TotalSeats { get; set; }

        /// <summary>
        /// Represents current available seats for booking
        /// </summary>
        public int AvailableSeats { get; set; }

        /// <summary>
        /// Try to reserve seats (if they are available)
        /// <para/>If not, <see langword="false"/> will be returned
        /// </summary>
        public bool TryReserveSeats(int count = 1)
        {
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(count, 0, nameof(count));
            int result = this.AvailableSeats - count;
            if (result >= 0)
            {
                this.AvailableSeats = result;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Try to release seats
        /// </summary>
        public bool ReleaseSeats(int count = 1)
        {
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(count, 0, nameof(count));
            int result = this.AvailableSeats + count;
            if (result <= this.TotalSeats)
            {
                this.AvailableSeats = result;
                return true;
            }
            return false;
        }
    }
}
