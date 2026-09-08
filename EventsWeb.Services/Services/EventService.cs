using EventsWeb.Core.DTO;
using EventsWeb.Core.Entities;
using EventsWeb.Core.Interface;

namespace EventsWeb.Services.Services
{
    public sealed class EventService : IEventService
    {
        /// <summary>
        /// Source collection
        /// </summary>
        private List<Event> events { get; init; }
        public EventService()
        {
            this.events = new();
        }
        public void Create(EventDto dto)
        {
            Event result = new();
            result.Id = Guid.NewGuid();
            result.Title = dto.Title;
            result.Description = dto.Description;
            result.StartAt = dto.StartAt;
            result.EndAt = dto.EndAt;
            this.events.Add(result);
        }

        public void Delete(Event entity)
        {
            this.events.Remove(entity);
        }

        public Event? GetById(Guid id)
        {
            return this.events.FirstOrDefault(e => e.Id == id);
        }

        public IReadOnlyCollection<Event> GetEvents()
        {
            return this.events;
        }

        public void Update(Event target, EventDto dto)
        {
            target.Title = dto.Title;
            target.Description = dto.Description;
            target.StartAt = dto.StartAt;
            target.EndAt = dto.EndAt;
        }
    }
}
