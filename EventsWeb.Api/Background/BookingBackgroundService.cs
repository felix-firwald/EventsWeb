using EventsWeb.Core.Entities;
using EventsWeb.Core.Interface;
using System.Threading.Tasks;

namespace EventsWeb.Api.Background
{
    public sealed class BookingBackgroundService : BackgroundService
    {
        private readonly SemaphoreSlim semaphore = new(1, 1);
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
                    IEnumerable<Task> tasks = pendingBookings.Select(b => this.ProcessBookingAsync(b, stoppingToken));
                    await Task.WhenAll(tasks);                    
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
        private async Task ProcessBookingAsync(Booking booking, CancellationToken stoppingToken)
        {
            await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
            await this.semaphore.WaitAsync(stoppingToken);
            try
            {
                await this.bookingService.Confirm(booking.Id);
            }
            finally
            {
                this.semaphore.Release();
            }
        }
    }
}
