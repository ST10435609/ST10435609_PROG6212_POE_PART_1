// start of Result entity
namespace RaceDay.Api.Models
{
    public class Result
    {

        public int ResultId { get; set; }


        public int EnrolmentId { get; set; }

        // maps to the SQL TIME column
        public TimeOnly FinishTime { get; set; }

        // maps to the SQL INT column
        public int FinishingPosition { get; set; }

        // navigation property: one result belongs to one enrolment
        public Enrolment Enrolment { get; set; } = null!;
    }
}
// end of Result entity