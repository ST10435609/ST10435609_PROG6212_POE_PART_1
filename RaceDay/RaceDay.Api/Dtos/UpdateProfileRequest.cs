// start of UpdateProfileRequest
using System.ComponentModel.DataAnnotations;

namespace RaceDay.Api.Dtos
{
    public class UpdateProfileRequest
    {
        [Required, MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        // nullable: the user can leave this blank to keep their current picture
        public string? ProfilePictureUrl { get; set; }
    }
}
// end of UpdateProfileRequest