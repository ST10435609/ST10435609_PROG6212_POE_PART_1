// start of CreateResultRequest
using System.ComponentModel.DataAnnotations;

namespace RaceDay.Api.Dtos
{
    public class CreateResultRequest
    {
        [Required]
        public TimeOnly FinishTime { get; set; }

        [Required, Range(1, int.MaxValue)]
        public int FinishingPosition { get; set; }
    }
}
// end of CreateResultRequest