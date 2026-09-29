using EventsWeb.Core.DTO;
using EventsWeb.Core.Interface;
using EventsWeb.Services.Services;

namespace EventsWeb.UnitTests.Tests
{
    /// <summary>
    /// Provides common infrastructure for event service tests.
    /// </summary>
    public abstract class TestBase
    {
        /// <summary>
        /// Gets service instance used by the current test
        /// </summary>
        protected IEventService Service { get; }

        /// <summary>
        /// Gets source test data
        /// </summary>
        protected IReadOnlyCollection<EventDto> StartData { get; }

        /// <summary>
        /// Initializes test infrastructure
        /// </summary>
        protected TestBase()
        {
            this.Service = new EventService();
            this.StartData = this.GetStartData();
        }

        /// <summary>
        /// Fills the service with predefined test events
        /// </summary>
        protected void SeedService()
        {
            foreach (EventDto dto in this.StartData)
            {
                this.Service.Create(dto);
            }
        }

        private IReadOnlyCollection<EventDto> GetStartData()
        {
            List<EventDto> result =
            [
                new(
                    "Конференция X",
                    null,
                    new DateTime(2026, 10, 1, 18, 0, 0),
                    new DateTime(2026, 10, 1, 20, 0, 0)
                ),
                new(
                    "Конференция Y",
                    null,
                    new DateTime(2026, 10, 2, 18, 0, 0),
                    new DateTime(2026, 10, 2, 20, 0, 0)
                ),
                new(
                    "Конференция H",
                    null,
                    new DateTime(2026, 10, 5, 18, 0, 0),
                    new DateTime(2026, 10, 5, 20, 0, 0)
                ),
                new(
                    "Лекция: многопоточное программирование",
                    null,
                    new DateTime(2026, 10, 6, 18, 0, 0),
                    new DateTime(2026, 10, 6, 20, 0, 0)
                ),
                new(
                    "Лекция: многопоточное программирование 2",
                    null,
                    new DateTime(2026, 10, 7, 12, 0, 0),
                    new DateTime(2026, 10, 7, 13, 30, 0)
                ),
                new(
                    "Лекция: рефлексия и деревья выражений",
                    null,
                    new DateTime(2026, 11, 9, 18, 0, 0),
                    new DateTime(2026, 11, 9, 20, 0, 0)
                ),
                new(
                    "Лекция: введение в LINQ",
                    null,
                    new DateTime(2026, 11, 9, 18, 0, 0),
                    new DateTime(2026, 11, 9, 20, 0, 0)
                )
            ];

            return result;
        }
    }
}