namespace EventsWeb.Core.DTO
{
    /// <summary>
    /// Represents DTO service returns to controller and controller returns to world
    /// </summary>
    public sealed class PaginatedResult<TEntity> where TEntity : class
    {
        /// <summary>
        /// Represents total count of entities (by filter)
        /// </summary>
        public int TotalCount { get; init; }

        /// <summary>
        /// Represents number of entities current page contains
        /// </summary>
        public int CurrentPageSize { get; init; }

        /// <summary>
        /// Represents current page index (from 1 to N)
        /// </summary>
        public int CurrentPageNumber { get; init; }

        /// <summary>
        /// Collection of entities
        /// </summary>
        public IReadOnlyCollection<TEntity> Entities { get; init; } = [];
    }
}
