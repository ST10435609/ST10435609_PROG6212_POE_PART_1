// start of CreateEventRequest
using System.ComponentModel.DataAnnotations;

namespace RaceDay.Api.Dtos
{
    public class CreateEventRequest
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public DateOnly EventDate { get; set; }

        [Required, MaxLength(150)]
        public string Location { get; set; } = string.Empty;

        [Required, Range(0.01, 9999.99)]
        public decimal Distance { get; set; }

        // "Run", "Walk" or "Cycle" 
        public string EventType { get; set; } = string.Empty;

        public string? BannerImageUrl { get; set; }
    }
}
// end of CreateEventRequest