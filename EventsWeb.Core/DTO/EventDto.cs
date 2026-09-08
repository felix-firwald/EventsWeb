using System.ComponentModel.DataAnnotations;

namespace EventsWeb.Core.DTO
{
    public record EventDto(
        [Required] string Title, 
        string? Description,
        [Required] DateTime StartAt,
        [Required] DateTime EndAt);
}
