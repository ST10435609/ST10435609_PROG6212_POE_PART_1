// start of User entity
using System.ComponentModel.DataAnnotations;

namespace RaceDay.Api.Models
{
    public class User
    {

        public int UserId { get; set; }

        //the role as either 'Organiser' or 'Participant' 
        [MaxLength(20)]
        public string Role { get; set; } = string.Empty;

        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        // never store the original password, hash the password
        [MaxLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

       
        [MaxLength(255)]
        public string? ProfilePictureUrl { get; set; }

        public DateTime CreatedAt { get; set; }

        // navigation property: events this user organises (Organiser role)
        public List<Event> OrganisedEvents { get; set; } = new();

        // navigation property: enrolments this user has made (Participant role)
        public List<Enrolment> Enrolments { get; set; } = new();
    }
}
// end of User entity