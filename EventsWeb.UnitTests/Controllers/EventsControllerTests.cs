using EventsWeb.Api.Controllers;
using EventsWeb.Core.DTO;
using EventsWeb.Core.Entities;
using EventsWeb.Core.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace EventsWeb.UnitTests.Controllers
{
    /// <summary>
    /// Contains unit tests for EventsController
    /// </summary>
    public sealed class EventsControllerTests
    {
        [Fact]
        public void GetAll_ForwardsFiltersAndPaginationToService()
        {            
            Mock<IEventService> serviceMock = new(MockBehavior.Strict);
            string title = "lecture";
            DateTime from = new(2026, 10, 1);
            DateTime to = new(2026, 11, 1);
            int page = 2;
            int pageSize = 5;
            PaginatedResult<Event> serviceResult = new()
            {
                TotalCount = 0,
                CurrentPageNumber = page,
                CurrentPageSize = 0,
                Entities = []
            };
            serviceMock
                .Setup(service => service.GetEvents(
                    It.Is<PaginationRequestDto>(
                        pagination =>
                            pagination.CurrentPage == page &&
                            pagination.PageSize == pageSize),
                    title,
                    from,
                    to))
                .Returns(serviceResult);
            EventsController controller = new(serviceMock.Object);            
            ActionResult<PaginatedResult<Event>> result =
                controller.GetAll(
                    title,
                    from,
                    to,
                    page,
                    pageSize);            
            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Same(serviceResult, okResult.Value);
            serviceMock.Verify(
                service => service.GetEvents(
                    It.Is<PaginationRequestDto>(
                        pagination =>
                            pagination.CurrentPage == page &&
                            pagination.PageSize == pageSize),
                    title,
                    from,
                    to),
                Times.Once);
        }

        [Fact]
        public void Get_WithExistingId_ReturnsOk()
        {           
            Mock<IEventService> serviceMock = new(MockBehavior.Strict);
            Event entity = new()
            {
                Id = Guid.NewGuid(),
                Title = "Событие",
                StartAt = new DateTime(2026, 10, 1, 10, 0, 0),
                EndAt = new DateTime(2026, 10, 1, 12, 0, 0)
            };
            serviceMock
                .Setup(service => service.GetById(entity.Id))
                .Returns(entity);
            EventsController controller = new(serviceMock.Object);            
            IActionResult result = controller.Get(entity.Id);           
            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Same(entity, okResult.Value);
        }

        [Fact]
        public void Get_WithMissingId_ReturnsNotFound()
        {         
            Mock<IEventService> serviceMock = new(MockBehavior.Strict);
            Guid id = Guid.NewGuid();
            serviceMock
                .Setup(service => service.GetById(id))
                .Returns((Event?)null);
            EventsController controller = new(serviceMock.Object);           
            IActionResult result = controller.Get(id);            
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Post_WithValidData_CallsCreateAndReturnsCreated()
        {           
            Mock<IEventService> serviceMock = new(MockBehavior.Strict);
            EventDto dto = new(
                "Событие",
                null,
                new DateTime(2026, 10, 1, 10, 0, 0),
                new DateTime(2026, 10, 1, 12, 0, 0));
            serviceMock.Setup(service => service.Create(dto));
            EventsController controller = new(serviceMock.Object);            
            IActionResult result = controller.Post(dto);            
            Assert.IsType<CreatedResult>(result);
            serviceMock.Verify(
                service => service.Create(dto),
                Times.Once);
        }

        [Fact]
        public void Put_WithExistingId_CallsUpdateAndReturnsNoContent()
        {          
            Mock<IEventService> serviceMock = new(MockBehavior.Strict);
            Event entity = new()
            {
                Id = Guid.NewGuid(),
                Title = "Старое событие",
                StartAt = new DateTime(2026, 10, 1, 10, 0, 0),
                EndAt = new DateTime(2026, 10, 1, 12, 0, 0)
            };
            EventDto dto = new(
                "Новое событие",
                null,
                new DateTime(2026, 10, 2, 10, 0, 0),
                new DateTime(2026, 10, 2, 12, 0, 0));
            serviceMock
                .Setup(service => service.GetById(entity.Id))
                .Returns(entity);
            serviceMock.Setup(service => service.Update(entity, dto));
            EventsController controller = new(serviceMock.Object);            
            IActionResult result = controller.Put(entity.Id, dto);           
            Assert.IsType<NoContentResult>(result);
            serviceMock.Verify(
                service => service.Update(entity, dto),
                Times.Once);
        }

        [Fact]
        public void Put_WithMissingId_ReturnsNotFoundAndDoesNotCallUpdate()
        {       
            Mock<IEventService> serviceMock = new(MockBehavior.Strict);
            Guid id = Guid.NewGuid();
            EventDto dto = new(
                "Событие",
                null,
                new DateTime(2026, 10, 1, 10, 0, 0),
                new DateTime(2026, 10, 1, 12, 0, 0));
            serviceMock
                .Setup(service => service.GetById(id))
                .Returns((Event?)null);
            EventsController controller = new(serviceMock.Object);           
            IActionResult result = controller.Put(id, dto);           
            Assert.IsType<NotFoundResult>(result);
            serviceMock.Verify(
                service => service.Update(
                    It.IsAny<Event>(),
                    It.IsAny<EventDto>()),
                Times.Never);
        }

        [Fact]
        public void Delete_WithExistingId_CallsDeleteAndReturnsNoContent()
        {           
            Mock<IEventService> serviceMock = new(MockBehavior.Strict);
            Event entity = new()
            {
                Id = Guid.NewGuid(),
                Title = "Событие",
                StartAt = new DateTime(2026, 10, 1, 10, 0, 0),
                EndAt = new DateTime(2026, 10, 1, 12, 0, 0)
            };
            serviceMock
                .Setup(service => service.GetById(entity.Id))
                .Returns(entity);
            serviceMock.Setup(service => service.Delete(entity));
            EventsController controller = new(serviceMock.Object);            
            IActionResult result = controller.Delete(entity.Id);            
            Assert.IsType<NoContentResult>(result);
            serviceMock.Verify(service => service.Delete(entity), Times.Once);
        }
    }
}