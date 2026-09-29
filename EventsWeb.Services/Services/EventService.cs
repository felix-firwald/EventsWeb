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
            Event? e = this.events.FirstOrDefault(e => e.Id == id);
            if (e == null)
            {
                throw new NotFoundException();
            }
            return e;
        }

        public PaginatedResult<Event> GetEvents(PaginationRequestDto pagination, string? title = null, DateTime? from = null, DateTime? to = null)
        {
            IQueryable<Event> query = this.events.AsQueryable();
            if (!string.IsNullOrWhiteSpace(title))
            {
                title = title.ToLower();
                query = query.Where(e => e.Title.ToLower().Contains(title));
            }
            if (from.HasValue)
            {
                query = query.Where(e => e.StartAt >= from);
        }
            if (to.HasValue)
            {
                query = query.Where(e => e.EndAt <= to);
            }

            int commonCount = query.Count();
            Event[] events = query
                .Skip(pagination.SkipCount)
                .Take(pagination.PageSize)
                .ToArray();
            PaginatedResult<Event> result = new()
        {
                TotalCount = commonCount,
                CurrentPageNumber = pagination.CurrentPage,
                CurrentPageSize = events.Length,
                Entities = events,
            };
            return result;
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
