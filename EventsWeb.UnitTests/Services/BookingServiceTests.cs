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
        }

        [Fact]
        public async Task CreateBookingAsync_MultipleBookingsForSameEvent_HaveUniqueIds()
        {
            Event eventEntity = this.CreateEvent();

            Booking[] bookings =
            [
                await this.bookingService.CreateBookingAsync(eventEntity.Id),
                await this.bookingService.CreateBookingAsync(eventEntity.Id),
                await this.bookingService.CreateBookingAsync(eventEntity.Id)
            ];

            Assert.All(bookings, booking => Assert.Equal(eventEntity.Id, booking.EventId));
            Assert.Equal(bookings.Length, bookings.Select(booking => booking.Id).Distinct().Count());
            Assert.DoesNotContain(bookings, booking => booking.Id == Guid.Empty);
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

        private Event CreateEvent()
        {
            EventDto dto = new(
                "Событие для бронирования",
                null,
                new DateTime(2026, 12, 1, 10, 0, 0),
                new DateTime(2026, 12, 1, 12, 0, 0));
            this.eventService.Create(dto);

            return Assert.Single(
                this.eventService.GetEvents(new PaginationRequestDto(1, 10)).Entities);
        }
    }
}
