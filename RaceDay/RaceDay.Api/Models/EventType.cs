// start of EventType entity
using System.ComponentModel.DataAnnotations;

namespace RaceDay.Api.Models
{
    public class EventType
    {
        // EF Core treats <ClassName>Id as the primary key by convention
        public int EventTypeId { get; set; }

        [MaxLength(20)]
        public string EventTypeName { get; set; } = string.Empty;

        // navigation property: one event type has many events
        public List<Event> Events { get; set; } = new();
    }
}
// end of EventType entity