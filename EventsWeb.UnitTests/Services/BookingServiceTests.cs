using EventsWeb.Core.DTO;
using EventsWeb.Core.Entities;
using EventsWeb.Core.Enums;
using EventsWeb.Core.Exceptions;
using EventsWeb.Core.Interface;
using EventsWeb.Services.Services;

namespace EventsWeb.UnitTests.Services
{
    public sealed class BookingServiceTests
    {
        private readonly IEventService eventService = new EventService();
        private readonly IBookingService bookingService;

        public BookingServiceTests()
        {
            this.bookingService = new BookingService(this.eventService);
        }

        [Fact]
        public async Task CreateBookingAsync_WithExistingEvent_ReturnsPendingBooking()
        {
            Event eventEntity = this.CreateEvent();

            Booking booking = await this.bookingService.CreateBookingAsync(eventEntity.Id);

            Assert.NotEqual(Guid.Empty, booking.Id);
            Assert.Equal(eventEntity.Id, booking.EventId);
            Assert.Equal(BookingStatus.Pending, booking.Status);
            Assert.NotEqual(default, booking.CreatedAt);
            Assert.Null(booking.ProcessedAt);
            Assert.Equal(eventEntity.TotalSeats - 1, eventEntity.AvailableSeats);
        }

        [Fact]
        public async Task CreateBookingAsync_MultipleBookingsForSameEvent_HaveUniqueIds()
        {
            Event eventEntity = this.CreateEvent(3);

            Booking[] bookings =
            [
                await this.bookingService.CreateBookingAsync(eventEntity.Id),
                await this.bookingService.CreateBookingAsync(eventEntity.Id),
                await this.bookingService.CreateBookingAsync(eventEntity.Id)
            ];

            Assert.All(bookings, booking => Assert.Equal(eventEntity.Id, booking.EventId));
            Assert.Equal(bookings.Length, bookings.Select(booking => booking.Id).Distinct().Count());
            Assert.DoesNotContain(bookings, booking => booking.Id == Guid.Empty);
            Assert.Equal(0, eventEntity.AvailableSeats);
        }

        [Fact]
        public async Task CreateBookingAsync_WithoutAvailableSeats_ThrowsNoAvailableSeatsException()
        {
            Event eventEntity = this.CreateEvent(1);
            await this.bookingService.CreateBookingAsync(eventEntity.Id);

            NoAvailableSeatsException exception = await Assert.ThrowsAsync<NoAvailableSeatsException>(
                () => this.bookingService.CreateBookingAsync(eventEntity.Id));

            Assert.Equal("No available seats for this event", exception.Message);
        }

        [Fact]
        public async Task GetBookingByIdAsync_WithExistingId_ReturnsCorrectBooking()
        {
            Event eventEntity = this.CreateEvent();
            Booking expected = await this.bookingService.CreateBookingAsync(eventEntity.Id);

            Booking actual = await this.bookingService.GetBookingByIdAsync(expected.Id);

            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(eventEntity.Id, actual.EventId);
            Assert.Equal(expected.Status, actual.Status);
            Assert.Equal(expected.CreatedAt, actual.CreatedAt);
            Assert.Equal(expected.ProcessedAt, actual.ProcessedAt);
        }

        [Fact]
        public async Task GetBookingByIdAsync_AfterConfirm_ReturnsConfirmedStatus()
        {
            Event eventEntity = this.CreateEvent();
            Booking booking = await this.bookingService.CreateBookingAsync(eventEntity.Id);

            await this.bookingService.Confirm(booking.Id);

            Booking actual = await this.bookingService.GetBookingByIdAsync(booking.Id);
            Assert.Equal(BookingStatus.Confirmed, actual.Status);
            Assert.NotNull(actual.ProcessedAt);
        }

        [Fact]
        public async Task GetBookingByIdAsync_AfterReject_ReturnsRejectedStatus()
        {
            Event eventEntity = this.CreateEvent();
            Booking booking = await this.bookingService.CreateBookingAsync(eventEntity.Id);

            await this.bookingService.Reject(booking.Id);

            Booking actual = await this.bookingService.GetBookingByIdAsync(booking.Id);
            Assert.Equal(BookingStatus.Rejected, actual.Status);
            Assert.NotNull(actual.ProcessedAt);
        }

        [Fact]
        public void Confirm_ChangesStatusAndSetsProcessedAt()
        {
            Booking booking = new();

            booking.Confirm();

            Assert.Equal(BookingStatus.Confirmed, booking.Status);
            Assert.NotNull(booking.ProcessedAt);
        }

        [Fact]
        public void Reject_ChangesStatusAndSetsProcessedAt()
        {
            Booking booking = new();

            booking.Reject();

            Assert.Equal(BookingStatus.Rejected, booking.Status);
            Assert.NotNull(booking.ProcessedAt);
        }

        [Fact]
        public async Task RejectAndReleaseSeats_RestoresAvailableSeats()
        {
            Event eventEntity = this.CreateEvent(1);
            Booking booking = await this.bookingService.CreateBookingAsync(eventEntity.Id);

            await this.bookingService.Reject(booking.Id);
            eventEntity.ReleaseSeats();

            Assert.Equal(1, eventEntity.AvailableSeats);
        }

        [Fact]
        public async Task RejectAndReleaseSeats_AllowsCreatingNewBooking()
        {
            Event eventEntity = this.CreateEvent(1);
            Booking firstBooking = await this.bookingService.CreateBookingAsync(eventEntity.Id);
            await this.bookingService.Reject(firstBooking.Id);
            eventEntity.ReleaseSeats();

            Booking secondBooking = await this.bookingService.CreateBookingAsync(eventEntity.Id);

            Assert.Equal(BookingStatus.Pending, secondBooking.Status);
            Assert.Equal(0, eventEntity.AvailableSeats);
        }

        [Fact]
        public async Task CreateBookingAsync_WithMissingEvent_ThrowsNotFoundException()
        {
            await Assert.ThrowsAsync<NotFoundException>(
                () => this.bookingService.CreateBookingAsync(Guid.NewGuid()));
        }

        [Fact]
        public async Task CreateBookingAsync_WithDeletedEvent_ThrowsNotFoundException()
        {
            Event eventEntity = this.CreateEvent();
            this.eventService.Delete(eventEntity);

            await Assert.ThrowsAsync<NotFoundException>(
                () => this.bookingService.CreateBookingAsync(eventEntity.Id));
        }

        [Fact]
        public async Task GetBookingByIdAsync_WithMissingId_ThrowsNotFoundException()
        {
            await Assert.ThrowsAsync<NotFoundException>(
                () => this.bookingService.GetBookingByIdAsync(Guid.NewGuid()));
        }

        [Fact]
        public async Task CreateBookingAsync_ConcurrentRequests_DoesNotCreateOverbooking()
        {
            Event eventEntity = this.CreateEvent(5);
            Task<(Booking? Booking, NoAvailableSeatsException? Exception)>[] tasks = Enumerable
                .Range(0, 20)
                .Select(_ => Task.Run(async () =>
                {
                    try
                    {
                        Booking booking = await this.bookingService.CreateBookingAsync(eventEntity.Id);
                        return (Booking: booking, Exception: (NoAvailableSeatsException?)null);
                    }
                    catch (NoAvailableSeatsException ex)
                    {
                        return (Booking: (Booking?)null, Exception: ex);
                    }
                }))
                .ToArray();

            (Booking? Booking, NoAvailableSeatsException? Exception)[] results = await Task.WhenAll(tasks);

            Assert.Equal(5, results.Count(result => result.Booking != null));
            Assert.Equal(15, results.Count(result => result.Exception != null));
            Assert.Equal(0, eventEntity.AvailableSeats);
        }

        [Fact]
        public async Task CreateBookingAsync_ConcurrentRequests_CreateUniqueIds()
        {
            Event eventEntity = this.CreateEvent(10);
            Task<Booking>[] tasks = Enumerable
                .Range(0, 10)
                .Select(_ => Task.Run(() => this.bookingService.CreateBookingAsync(eventEntity.Id)))
                .ToArray();

            Booking[] bookings = await Task.WhenAll(tasks);

            Assert.Equal(10, bookings.Select(booking => booking.Id).Distinct().Count());
        }

        private Event CreateEvent(int totalSeats = 10)
        {
            CreateEventDto dto = new(
                "Событие для бронирования",
                null,
                new DateTime(2026, 12, 1, 10, 0, 0),
                new DateTime(2026, 12, 1, 12, 0, 0),
                totalSeats);
            this.eventService.Create(dto);

            return Assert.Single(
                this.eventService.GetEvents(new PaginationRequestDto(1, 10)).Entities);
        }
    }
}
