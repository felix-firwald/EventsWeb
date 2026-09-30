using EventsWeb.Core.DTO;
using EventsWeb.Core.Entities;
using EventsWeb.Core.Exceptions;
using EventsWeb.UnitTests.Tests;
using EventsWeb.Core.Interface;
using System.ComponentModel.DataAnnotations;

namespace EventsWeb.UnitTests.Services
{
    /// <summary>
    /// Contains unit tests for <see cref="IEventService"/>
    /// </summary>
    public sealed class EventServiceTests : TestBase
    {
        [Fact]
        public void Create_WithValidData_CreatesEvent()
        {        
            EventDto dto = new(
                "Новое событие",
                "Описание",
                new DateTime(2026, 12, 1, 10, 0, 0),
                new DateTime(2026, 12, 1, 12, 0, 0));            
            this.Service.Create(dto);            
            PaginatedResult<Event> result = this.Service.GetEvents(new PaginationRequestDto(1, 10));
            Event createdEvent = Assert.Single(result.Entities);
            Assert.NotEqual(Guid.Empty, createdEvent.Id);
            Assert.Equal(dto.Title, createdEvent.Title);
            Assert.Equal(dto.Description, createdEvent.Description);
            Assert.Equal(dto.StartAt, createdEvent.StartAt);
            Assert.Equal(dto.EndAt, createdEvent.EndAt);
        }

        [Fact]
        public void GetEvents_WithoutFilters_ReturnsAllEvents()
        {       
            this.SeedService();            
            PaginatedResult<Event> result = this.Service.GetEvents(new PaginationRequestDto(1, 20));            
            Assert.Equal(this.StartData.Count, result.TotalCount);
            Assert.Equal(this.StartData.Count, result.CurrentPageSize);
            Assert.Equal(1, result.CurrentPageNumber);
            Assert.Equal(
                this.StartData.Select(dto => dto.Title),
                result.Entities.Select(entity => entity.Title));
        }

        [Fact]
        public void GetById_WithExistingId_ReturnsEvent()
        {           
            this.SeedService();
            PaginatedResult<Event> allEvents = this.Service.GetEvents(new PaginationRequestDto(1, 20));
            Event expected = allEvents.Entities.First();           
            Event? result = this.Service.GetById(expected.Id);      
            Assert.NotNull(result);
            Assert.Same(expected, result);
        }

        [Fact]
        public void Update_WithExistingEvent_UpdatesEvent()
        {
            this.SeedService();
            PaginatedResult<Event> allEvents = this.Service.GetEvents(new PaginationRequestDto(1, 20));
            Event target = allEvents.Entities.First();
            Guid originalId = target.Id;
            EventDto dto = new(
                "Изменённое событие",
                "Изменённое описание",
                new DateTime(2026, 12, 10, 10, 0, 0),
                new DateTime(2026, 12, 10, 13, 0, 0));            
            this.Service.Update(target, dto);           
            Assert.Equal(originalId, target.Id);
            Assert.Equal(dto.Title, target.Title);
            Assert.Equal(dto.Description, target.Description);
            Assert.Equal(dto.StartAt, target.StartAt);
            Assert.Equal(dto.EndAt, target.EndAt);
        }

        [Fact]
        public void Delete_WithExistingEvent_DeletesEvent()
        {           
            this.SeedService();
            PaginatedResult<Event> allEvents = this.Service.GetEvents(new PaginationRequestDto(1, 20));
            Event target = allEvents.Entities.First();
            this.Service.Delete(target);
            PaginatedResult<Event> result = this.Service.GetEvents(new PaginationRequestDto(1, 20));
            Assert.Equal(this.StartData.Count - 1, result.TotalCount);
            Assert.DoesNotContain(result.Entities, entity => entity.Id == target.Id);
        }

        [Fact]
        public void GetEvents_WithTitleFilter_ReturnsCaseInsensitivePartialMatches()
        {
            this.SeedService();
            PaginatedResult<Event> result = this.Service.GetEvents(new PaginationRequestDto(1, 20), title: "конференция");
            Assert.Equal(3, result.TotalCount);
            Assert.All(
                result.Entities,
                entity => Assert.True(
                    entity.Title.Contains(
                        "конференция",
                        StringComparison.OrdinalIgnoreCase)));
        }

        [Fact]
        public void GetEvents_WithStartDateFilter_ReturnsEventsStartingNotEarlierThanDate()
        {
            this.SeedService();
            DateTime from = new(2026, 11, 1);
            PaginatedResult<Event> result = this.Service.GetEvents(
                new PaginationRequestDto(1, 20),
                from: from);
            Assert.Equal(2, result.TotalCount);
            Assert.All(
                result.Entities,
                entity => Assert.True(entity.StartAt >= from));
        }

        [Fact]
        public void GetEvents_WithEndDateFilter_ReturnsEventsEndingNotLaterThanDate()
        {
            this.SeedService();
            DateTime to = new(2026, 10, 5, 23, 59, 59);
            PaginatedResult<Event> result = this.Service.GetEvents(
                new PaginationRequestDto(1, 20),
                to: to);
            Assert.Equal(3, result.TotalCount);
            Assert.All(result.Entities, entity => Assert.True(entity.EndAt <= to));
        }

        [Fact]
        public void GetEvents_WithPagination_ReturnsRequestedPage()
        {
            this.SeedService();
            PaginatedResult<Event> result = this.Service.GetEvents(new PaginationRequestDto(2, 3));
            string[] expectedTitles =
            [
                "Лекция: многопоточное программирование",
                "Лекция: многопоточное программирование 2",
                "Лекция: рефлексия и деревья выражений"
            ];
            Assert.Equal(7, result.TotalCount);
            Assert.Equal(2, result.CurrentPageNumber);
            Assert.Equal(3, result.CurrentPageSize);
            Assert.Equal(expectedTitles, result.Entities.Select(entity => entity.Title));
        }

        [Fact]
        public void GetEvents_WithCombinedFilters_ReturnsEventsMatchingAllFilters()
        {
            this.SeedService();
            DateTime from = new(2026, 10, 7);
            DateTime to = new(2026, 11, 9, 20, 0, 0);
            PaginatedResult<Event> result = this.Service.GetEvents(
                new PaginationRequestDto(1, 20),
                title: "Лекция",
                from: from,
                to: to);
            string[] expectedTitles =
            [
                "Лекция: многопоточное программирование 2",
                "Лекция: рефлексия и деревья выражений",
                "Лекция: введение в LINQ"
            ];
            Assert.Equal(3, result.TotalCount);
            Assert.Equal(expectedTitles, result.Entities.Select(entity => entity.Title));
        }

        [Fact]
        public void GetById_WithMissingId_ThrowsNotFoundException()
        {
            this.SeedService();
            Guid id = Guid.NewGuid();
            Assert.Throws<NotFoundException>(() => this.Service.GetById(id));
        }

        [Fact]
        public void Create_WithEmptyTitle_ThrowsValidationException()
        {
            EventDto dto = new(
                "   ",
                null,
                new DateTime(2026, 12, 1, 10, 0, 0),
                new DateTime(2026, 12, 1, 12, 0, 0));
            Assert.Throws<ValidationException>(() => this.Service.Create(dto));
        }

        [Fact]
        public void Create_WithEndBeforeStart_ThrowsValidationException()
        {
            EventDto dto = new(
                "Некорректное событие",
                null,
                new DateTime(2026, 12, 1, 12, 0, 0),
                new DateTime(2026, 12, 1, 10, 0, 0));
            Assert.Throws<ValidationException>(() => this.Service.Create(dto));
        }

        [Fact]
        public void Update_WithEndBeforeStart_ThrowsValidationException()
        {
            this.SeedService();
            PaginatedResult<Event> allEvents = this.Service.GetEvents(new PaginationRequestDto(1, 20));
            Event target = allEvents.Entities.First();
            EventDto dto = new(
                "Некорректное обновление",
                null,
                new DateTime(2026, 12, 1, 12, 0, 0),
                new DateTime(2026, 12, 1, 10, 0, 0));
            Assert.Throws<ValidationException>(() => this.Service.Update(target, dto));
        }
    }
}