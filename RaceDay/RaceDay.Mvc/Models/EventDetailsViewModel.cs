// start of EventDetailsViewModel
namespace RaceDay.Mvc.Models
{
    public class EventDetailsViewModel
    {
        public int EventId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateOnly EventDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public decimal Distance { get; set; }
        public string EventType { get; set; } = string.Empty;
        public string? BannerImageUrl { get; set; }
        public List<CategoryViewModel> Categories { get; set; } = new();
    }
}
// end of EventDetailsViewModel