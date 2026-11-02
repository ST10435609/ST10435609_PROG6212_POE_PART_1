// start of EventListItemViewModel
namespace RaceDay.Mvc.Models
{
    public class EventListItemViewModel
    {
        public int EventId { get; set; }
        public int OrganiserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateOnly EventDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public decimal Distance { get; set; }
        public string EventType { get; set; } = string.Empty;
        public int EnrolmentCount { get; set; }
    }
}
// end of EventListItemViewModel