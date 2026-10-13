// start of CreateEnrolmentRequest
using System.ComponentModel.DataAnnotations;

namespace RaceDay.Api.Dtos
{
    public class CreateEnrolmentRequest
    {
        [Required]
        public int CategoryId { get; set; }
    }
}
// end of CreateEnrolmentRequest