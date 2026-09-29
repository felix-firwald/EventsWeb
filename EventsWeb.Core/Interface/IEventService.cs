using EventsWeb.Core.DTO;
using EventsWeb.Core.Entities;

namespace EventsWeb.Core.Interface
{
    /// <summary>
    /// Service working with <see cref="Event"/> entities
    /// </summary>
    public interface IEventService
    {
        /// <summary>
        /// Get all events
        /// </summary>
        public PaginatedResult<Event> GetEvents(PaginationRequestDto pagination, string? title = null, DateTime? from = null, DateTime? to = null);

        /// <summary>
        /// Gets an event by its id if exists, otherwise null
        /// </summary>
        public Event? GetById(Guid id);

        /// <summary>
        /// Creates new event
        /// </summary>
        public void Create(EventDto dto);

        /// <summary>
        /// Updates existing event
        /// </summary>
        public void Update(Event target, EventDto dto);

        /// <summary>
        /// Deletes existing event
        /// </summary>
        public void Delete(Event entity);

    }
}
