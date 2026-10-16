// start of Category entity
using System.ComponentModel.DataAnnotations;

namespace RaceDay.Api.Models
{
    public class Category
    {
 
        public int CategoryId { get; set; }

 
        public int EventId { get; set; }

        [MaxLength(50)]
        public string CategoryName { get; set; } = string.Empty;

        // navigation property: one category belongs to one event
        public Event Event { get; set; } = null!;

        // navigation property: one category has many enrolments
        public List<Enrolment> Enrolments { get; set; } = new();
    }
}
// end of Category entity