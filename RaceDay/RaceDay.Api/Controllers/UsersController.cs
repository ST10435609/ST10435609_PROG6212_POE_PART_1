using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Dtos;
using RaceDay.Api.Security;

namespace RaceDay.Api.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ApiControllerBase
    {
        private readonly RaceDayDbContext _db;

        public UsersController(RaceDayDbContext db)
        {
            _db = db;
        }

        [HttpGet("me")]
        [SessionAuthorize]
        public async Task<IActionResult> GetMyProfile()
        {
            // 1. Find the logged-in user
            var user = await _db.Users.FindAsync(CurrentUserId);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            // 2. Map the user to the response DTO
            var response = new UserProfileResponse
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                ProfilePictureUrl = user.ProfilePictureUrl
            };

            return Ok(response);
        }

        [HttpPut("me")]
        [SessionAuthorize]
        public async Task<IActionResult> UpdateMyProfile(
            UpdateProfileRequest request)
        {
            // 1. Find the logged-in user
            var user = await _db.Users.FindAsync(CurrentUserId);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            // 2. Check whether another user already uses this email
            bool emailExists = await _db.Users.AnyAsync(
                u => u.Email == request.Email &&
                     u.UserId != CurrentUserId);

            if (emailExists)
            {
                return Conflict("Email already registered.");
            }

            // 3. Update the user's profile
            user.FullName = request.FullName;
            user.Email = request.Email;

            // Keep the current picture if none was supplied
            if (!string.IsNullOrWhiteSpace(request.ProfilePictureUrl))
            {
                user.ProfilePictureUrl = request.ProfilePictureUrl;
            }

            // 4. Save changes
            await _db.SaveChangesAsync();

            // 5. Return the updated profile
            var response = new UserProfileResponse
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                ProfilePictureUrl = user.ProfilePictureUrl
            };

            return Ok(response);
        }
    }
}
