// start of AuthResponse
namespace RaceDay.Api.Dtos
{
    public class AuthResponse
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
// end of AuthResponse