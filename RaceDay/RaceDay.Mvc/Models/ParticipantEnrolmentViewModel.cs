// start of ParticipantEnrolmentViewModel
namespace RaceDay.Mvc.Models
{
    public class ParticipantEnrolmentViewModel
    {
        public int EnrolmentId { get; set; }
        public int EventId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public DateTime EnrolmentDate { get; set; }
        public string EnrolmentStatus { get; set; } = string.Empty;
    }
}
// end of ParticipantEnrolmentViewModel