// start of CategoryFormViewModel
using System.ComponentModel.DataAnnotations;

namespace RaceDay.Mvc.Models
{
    public class CategoryFormViewModel
    {
        public int CategoryId { get; set; }
        public int EventId { get; set; }

        [Required, MaxLength(50)]
        public string CategoryName { get; set; } = string.Empty;
    }
}
// end of CategoryFormViewModel