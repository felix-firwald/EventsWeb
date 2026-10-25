using EventsWeb.Core.Entities;
using EventsWeb.Core.Interface;

namespace EventsWeb.Api.Background
{
    public sealed class BookingBackgroungService : BackgroundService
    {
        private IBookingService bookingService { get; }
        public BookingBackgroungService(IBookingService bookingService)
        {
            this.bookingService = bookingService;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                IEnumerable<Booking> pendingBookings = await this.bookingService.GetPending();
                foreach (Booking booking in pendingBookings)
                {
                    stoppingToken.ThrowIfCancellationRequested();
                    await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
                    await this.bookingService.Confirm(booking.Id);
                }
            }
        }
    }
}
