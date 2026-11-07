// start of ParticipantResultViewModel
namespace RaceDay.Mvc.Models
{
    public class ParticipantResultViewModel
    {
        public int ResultId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public TimeOnly FinishTime { get; set; }
        public int FinishingPosition { get; set; }
    }
}
// end of ParticipantResultViewModel