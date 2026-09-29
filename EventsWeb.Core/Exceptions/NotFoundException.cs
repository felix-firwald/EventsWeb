namespace EventsWeb.Core.Exceptions
{
    /// <summary>
    /// Represents exception throws when entity with concrete ID not found
    /// </summary>
    public class NotFoundException : Exception
    {
        public NotFoundException()
        {
        }

        public NotFoundException(string? message) : base(message)
        {
        }
    }
}
