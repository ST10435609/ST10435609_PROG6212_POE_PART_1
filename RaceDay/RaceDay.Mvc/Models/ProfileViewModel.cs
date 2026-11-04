// start of ProfileViewModel
using System.ComponentModel.DataAnnotations;

namespace RaceDay.Mvc.Models
{
    public class ProfileViewModel
    {
        [Required, MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        // TODO: replace with a real file upload once Azure Blob Storage is wired in
        public string? ProfilePictureUrl { get; set; }
    }
}
// end of ProfileViewModel