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
        private readonly RaceDay.Api.Services.BlobStorageService _blob_storage_service;

        public UsersController(
            RaceDayDbContext db,
            RaceDay.Api.Services.BlobStorageService blob_storage_service)
        {
            _db = db;
            _blob_storage_service = blob_storage_service;
        }

        // POST /api/users/me/profile-picture — any logged-in user
        [HttpPost("me/profile-picture")]
        [SessionAuthorize]
        public async Task<IActionResult> UploadProfilePicture(IFormFile file)
        {
            var user = await _db.Users.FindAsync(CurrentUserId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            if (file.Length > 5 * 1024 * 1024)
            {
                return BadRequest("File too large. Maximum size is 5MB.");
            }

            var allowed_types = new[] { "image/jpeg", "image/png", "image/webp" };
            if (!allowed_types.Contains(file.ContentType))
            {
                return BadRequest("Only JPEG, PNG or WEBP images are allowed.");
            }

            var url = await _blob_storage_service.UploadAsync("profile-pictures", file);
            user.ProfilePictureUrl = url;
            await _db.SaveChangesAsync();

            return Ok(new { profilePictureUrl = url });
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
