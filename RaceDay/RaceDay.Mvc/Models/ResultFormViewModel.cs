// start of ResultFormViewModel
using System.ComponentModel.DataAnnotations;

namespace RaceDay.Mvc.Models
{
    public class ResultFormViewModel
    {
        public int EnrolmentId { get; set; }

        [Required]
        public TimeOnly FinishTime { get; set; }

        [Required, Range(1, int.MaxValue)]
        public int FinishingPosition { get; set; }
    }
}
// end of ResultFormViewModel