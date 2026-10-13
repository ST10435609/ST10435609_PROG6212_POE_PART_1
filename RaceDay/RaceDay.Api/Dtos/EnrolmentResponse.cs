// start of EnrolmentResponse
namespace RaceDay.Api.Dtos
{
    public class EnrolmentResponse
    {
        public int EnrolmentId { get; set; }
        public int ParticipantId { get; set; }
        public int EventId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public DateTime EnrolmentDate { get; set; }
        public string EnrolmentStatus { get; set; } = string.Empty;
    }
}
// end of EnrolmentResponse