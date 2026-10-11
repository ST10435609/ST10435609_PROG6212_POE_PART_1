// start of Event entity
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.Api.Models
{
    public class Event
    {

        public int EventId { get; set; }

        public int OrganiserId { get; set; }

        // foreign key: Run, Walk or Cycle
        public int EventTypeId { get; set; }

        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public DateOnly EventDate { get; set; }

        [MaxLength(150)]
        public string Location { get; set; } = string.Empty;

        [Column(TypeName = "decimal(5,2)")]
        public decimal Distance { get; set; }

        // nullable: image is optional, stored later in Azure Blob Storage
        [MaxLength(255)]
        public string? BannerImageUrl { get; set; }

        // navigation property: one event belongs to one organiser
        public User Organiser { get; set; } = null!;

        // navigation property: one event belongs to one event type
        public EventType EventType { get; set; } = null!;

        // navigation property: one event has many categories
        public List<Category> Categories { get; set; } = new();

        // navigation property: one event receives many enrolments
        public List<Enrolment> Enrolments { get; set; } = new();
    }
}
// end of Event entity