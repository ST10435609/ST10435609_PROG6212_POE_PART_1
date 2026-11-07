// start of OrganiserDashboardViewModel
namespace RaceDay.Mvc.Models
{
    public class OrganiserDashboardViewModel
    {
        public List<EventListItemViewModel> Events { get; set; } = new();
        public int TotalEvents => Events.Count;
        public int TotalEnrolments => Events.Sum(e => e.EnrolmentCount);
    }
}
// end of OrganiserDashboardViewModel