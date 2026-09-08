namespace EventsWeb.Core.DTO
{
    public record EventDto(
        string Title, 
        string Description, 
        DateTime StartAt, 
        DateTime EndAt);
}
