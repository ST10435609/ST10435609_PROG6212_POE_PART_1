using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Dtos;
using RaceDay.Api.Models;
using RaceDay.Api.Security;

namespace RaceDay.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly RaceDayDbContext _db;

        public AuthController(RaceDayDbContext db)
        {
            _db = db;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            // 1. Validate the role
            if (request.Role != "Organiser" &&
                request.Role != "Participant")
            {
                return BadRequest("Invalid role. Use Organiser or Participant.");
            }

            // 2. Check whether the email already exists
            if (await _db.Users.AnyAsync(u => u.Email == request.Email))
            {
                return Conflict("Email already registered.");
            }

            // 3. Hash the password
            string passwordHash =
                BCrypt.Net.BCrypt.HashPassword(request.Password);

            // 4. Create the user
            var user = new User
            {
                FullName = request.FullName,
                Email = request.Email,
                PasswordHash = passwordHash,
                Role = request.Role
            };

            _db.Users.Add(user);

            await _db.SaveChangesAsync();

            // 5. Return the response without the password hash
            var response = new AuthResponse
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Role = user.Role
            };

            return StatusCode(201, response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            // 1. Find the user by email
            var user = await _db.Users.FirstOrDefaultAsync(
                u => u.Email == request.Email);

            // 2. Verify the password
            if (user == null ||
                !BCrypt.Net.BCrypt.Verify(
                    request.Password,
                    user.PasswordHash))
            {
                return Unauthorized("Invalid email or password.");
            }

            // 3. Store the user details in the session
            HttpContext.Session.SetInt32(
                SessionKeys.UserId,
                user.UserId);

            HttpContext.Session.SetString(
                SessionKeys.Role,
                user.Role);

            // 4. Return the response
            var response = new AuthResponse
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Role = user.Role
            };

            return Ok(response);
        }

        [HttpPost("logout")]
        [SessionAuthorize]
        public IActionResult Logout()
        {
            // Clear the session
            HttpContext.Session.Clear();

            return Ok("Logged out successfully.");
        }
    }
}