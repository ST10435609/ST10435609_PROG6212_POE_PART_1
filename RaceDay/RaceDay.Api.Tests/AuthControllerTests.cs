// start of AuthControllerTests
using System.Net;
using System.Net.Http.Json;
using RaceDay.Api.Dtos;
using Xunit;

namespace RaceDay.Api.Tests
{
    public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;

        public AuthControllerTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Register_WithNewEmail_ReturnsCreated()
        {
            var client = _factory.CreateClient();
            var email = $"organiser_{Guid.NewGuid()}@test.com";

            var response = await client.PostAsJsonAsync("/api/auth/register", new RegisterRequest
            {
                FullName = "Thabo Nkosi",
                Email = email,
                Password = "Password123",
                Role = "Organiser"
            });

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task Register_WithDuplicateEmail_ReturnsConflict()
        {
            var client = _factory.CreateClient();
            var email = $"duplicate_{Guid.NewGuid()}@test.com";

            var request = new RegisterRequest
            {
                FullName = "Lindiwe Mokoena",
                Email = email,
                Password = "Password123",
                Role = "Organiser"
            };

            await client.PostAsJsonAsync("/api/auth/register", request);
            var secondResponse = await client.PostAsJsonAsync("/api/auth/register", request);

            Assert.Equal(HttpStatusCode.Conflict, secondResponse.StatusCode);
        }

        [Fact]
        public async Task Login_WithCorrectCredentials_ReturnsOk()
        {
            var client = _factory.CreateClient();
            var email = $"login_{Guid.NewGuid()}@test.com";

            await client.PostAsJsonAsync("/api/auth/register", new RegisterRequest
            {
                FullName = "Sipho Dlamini",
                Email = email,
                Password = "Password123",
                Role = "Participant"
            });

            var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest
            {
                Email = email,
                Password = "Password123"
            });

            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        }

        [Fact]
        public async Task Login_WithWrongPassword_ReturnsUnauthorized()
        {
            var client = _factory.CreateClient();
            var email = $"wrongpass_{Guid.NewGuid()}@test.com";

            await client.PostAsJsonAsync("/api/auth/register", new RegisterRequest
            {
                FullName = "Naledi Khumalo",
                Email = email,
                Password = "Password123",
                Role = "Participant"
            });

            var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest
            {
                Email = email,
                Password = "WrongPassword"
            });

            Assert.Equal(HttpStatusCode.Unauthorized, loginResponse.StatusCode);
        }
    }
}
// end of AuthControllerTests