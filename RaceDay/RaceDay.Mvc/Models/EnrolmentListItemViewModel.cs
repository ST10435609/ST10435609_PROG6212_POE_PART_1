// start of EnrolmentListItemViewModel
namespace RaceDay.Mvc.Models
{
    public class EnrolmentListItemViewModel
    {
        public int EnrolmentId { get; set; }
        public int ParticipantId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public DateTime EnrolmentDate { get; set; }
        public string EnrolmentStatus { get; set; } = string.Empty;
    }
}
// end of EnrolmentListItemViewModel