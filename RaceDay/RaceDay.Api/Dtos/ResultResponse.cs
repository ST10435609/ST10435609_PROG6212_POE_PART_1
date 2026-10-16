// start of ResultResponse
namespace RaceDay.Api.Dtos
{
    public class ResultResponse
    {
        public int ResultId { get; set; }
        public int EnrolmentId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public TimeOnly FinishTime { get; set; }
        public int FinishingPosition { get; set; }
    }
}
// end of ResultResponse