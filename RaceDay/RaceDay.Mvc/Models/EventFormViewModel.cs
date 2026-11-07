// start of EventFormViewModel
using System.ComponentModel.DataAnnotations;

namespace RaceDay.Mvc.Models
{
    public class EventFormViewModel
    {
        public int EventId { get; set; }

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

        [Required]
        public string EventType { get; set; } = string.Empty;

        // TODO: replace with a real file upload once Azure Blob Storage is

        public string? BannerImageUrl { get; set; }
    }
}
// end of EventFormViewModel