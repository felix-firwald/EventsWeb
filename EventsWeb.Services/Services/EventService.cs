using EventsWeb.Core.DTO;
using EventsWeb.Core.Entities;
using EventsWeb.Core.Exceptions;
using EventsWeb.Core.Interface;
using System.ComponentModel.DataAnnotations;

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
            this.events = [];
        }
        public void Create(EventDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
            {
                throw new ValidationException("Событие не может не содержать наименования");
            }
            if (dto.StartAt > dto.EndAt)
            {
                throw new ValidationException("Дата начала события не может быть больше даты окончания");
            }
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

        public Event GetById(Guid id)
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
            if (dto.StartAt > dto.EndAt)
            {
                throw new ValidationException("Дата начала события не может быть после даты его окончания");
            }
            target.Title = dto.Title;
            target.Description = dto.Description;
            target.StartAt = dto.StartAt;
            target.EndAt = dto.EndAt;
        }
    }
}
