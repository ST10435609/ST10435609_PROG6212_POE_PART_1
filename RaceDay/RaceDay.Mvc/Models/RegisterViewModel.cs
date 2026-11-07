// start of RegisterViewModel
using System.ComponentModel.DataAnnotations;

namespace RaceDay.Mvc.Models
{
    public class RegisterViewModel
    {
        [Required, MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(8), DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = string.Empty;
    }
}
// end of RegisterViewModel