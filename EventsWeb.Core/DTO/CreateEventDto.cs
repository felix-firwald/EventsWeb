using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EventsWeb.Core.DTO
{
    public record CreateEventDto(
        [Required] string Title,
        string? Description,
        [Required] DateTime StartAt,
        [Required] DateTime EndAt,
        [Required] int? TotalSeats);
}
