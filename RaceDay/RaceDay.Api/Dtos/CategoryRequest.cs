// start of CategoryRequest
using System.ComponentModel.DataAnnotations;

namespace RaceDay.Api.Dtos
{
    public class CategoryRequest
    {
        [Required, MaxLength(50)]
        public string CategoryName { get; set; } = string.Empty;
    }
}
// end of CategoryRequest