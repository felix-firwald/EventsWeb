using EventsWeb.Core.Entities;
using EventsWeb.Core.Interface;

namespace EventsWeb.Api.Background
{
    public sealed class BookingBackgroundService : BackgroundService
    {
        private readonly SemaphoreSlim semaphore = new(1, 1);
        private IBookingService bookingService { get; }
        private IEventService eventService { get; }
        private ILogger<BookingBackgroundService> logger { get; }
        private TimeSpan pollingLatency { get; }
        public BookingBackgroundService(ILogger<BookingBackgroundService> logger, IEventService eventService, IBookingService bookingService)
        {
            this.bookingService = bookingService;
            this.eventService = eventService;
            this.logger = logger;
            this.pollingLatency = TimeSpan.FromSeconds(2);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            this.logger.LogInformation("Бекграунд-сервис подтверждения бронирования запущен");
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    IEnumerable<Booking> pendingBookings = await this.bookingService.GetPending();
                    if (pendingBookings.Any())
                    {
                        IEnumerable<Task> tasks = pendingBookings.Select(b => this.ProcessBookingAsync(b, stoppingToken));
                        await Task.WhenAll(tasks);
                    }
                    else
                    {
                        await Task.Delay(this.pollingLatency, stoppingToken);
                    }
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
            await Task.Delay(this.pollingLatency, stoppingToken);
            await this.semaphore.WaitAsync(stoppingToken);
            Event? eventEntity = null;
            try
            {
                if (this.eventService.Exists(booking.EventId))
                {
                    eventEntity = this.eventService.GetById(booking.EventId);
                    await this.bookingService.Confirm(booking.Id);
                }
                else
                {
                    await this.bookingService.Reject(booking.Id);
                    this.logger.LogWarning("Событие для бронирования {BookingId} было удалено", booking.Id);
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Ошибка обработки бронирования {BookingId}", booking.Id);
                await this.bookingService.Reject(booking.Id);
                if (eventEntity != null)
                {
                    eventEntity.ReleaseSeats();
                }
            }
            finally
            {
                this.semaphore.Release();
            }
        }
    }
}
