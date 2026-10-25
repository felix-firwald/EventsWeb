using EventsWeb.Core.Entities;
using EventsWeb.Core.Interface;

namespace EventsWeb.Api.Background
{
    public sealed class BookingBackgroundService : BackgroundService
    {
        private IBookingService bookingService { get; }
        private ILogger<BookingBackgroundService> logger { get; }
        public BookingBackgroundService(ILogger<BookingBackgroundService> logger, IBookingService bookingService)
        {
            this.bookingService = bookingService;
            this.logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            this.logger.LogInformation("Бекграунд-сервис подтверждения бронирования запущен");
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    IEnumerable<Booking> pendingBookings = await this.bookingService.GetPending();
                    foreach (Booking booking in pendingBookings)
                    {
                        stoppingToken.ThrowIfCancellationRequested();                      
                        await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
                        await this.bookingService.Confirm(booking.Id);
                    }
                    await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    this.logger.LogError(ex, "Ошибка выполнения подтверждения бронирований");
                }
            }
            this.logger.LogInformation("Бекграунд-сервис подтверждения бронирования остановлен");
        }
    }
}
