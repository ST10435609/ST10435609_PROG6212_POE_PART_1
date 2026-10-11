// start of Enrolment entity
using System.ComponentModel.DataAnnotations;

namespace RaceDay.Api.Models
{
    public class Enrolment
    {

        public int EnrolmentId { get; set; }

        public int ParticipantId { get; set; }


        public int EventId { get; set; }

        public int CategoryId { get; set; }

        public DateTime EnrolmentDate { get; set; }

        [MaxLength(20)]
        public string EnrolmentStatus { get; set; } = "Confirmed";


        public User Participant { get; set; } = null!;

        // navigation property: the event
        public Event Event { get; set; } = null!;

        // navigation property: the chosen category
        public Category Category { get; set; } = null!;

        // navigation property: a result may not exist yet
        public Result? Result { get; set; }
    }
}
// end of Enrolment entity