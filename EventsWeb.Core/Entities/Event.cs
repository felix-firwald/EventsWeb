namespace EventsWeb.Core.Entities
{
    public class Event
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public required int TotalSeats { get; set; }
        public int AvailableSeats { get; set; }
        public bool TryReserveSeats(int count = 1)
        {
            int result = this.AvailableSeats - count;
            if (result >= 0)
            {
                this.AvailableSeats = result;
                return true;
            }
            return false;
        }
    }
}
