// start of EventsControllerTests
using System.Net;
using System.Net.Http.Json;
using RaceDay.Api.Dtos;
using Xunit;

namespace RaceDay.Api.Tests
{
    public class EventsControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;

        public EventsControllerTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetEvents_WithoutLogin_IsPublicAndReturnsOk()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/api/events");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task CreateEvent_AsOrganiser_ReturnsCreated()
        {
            var client = _factory.CreateClient();
            await TestHelpers.RegisterAndLoginAsync(
                client, $"org_{Guid.NewGuid()}@test.com", "Password123", "Organiser");

            var response = await client.PostAsJsonAsync("/api/events", new CreateEventRequest
            {
                Name = "Soweto Heritage Walk",
                EventDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(1)),
                Location = "Soweto",
                Distance = 5.00m,
                EventType = "Walk"
            });

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task CreateEvent_AsParticipant_ReturnsForbidden()
        {
            var client = _factory.CreateClient();
            await TestHelpers.RegisterAndLoginAsync(
                client, $"part_{Guid.NewGuid()}@test.com", "Password123", "Participant");

            var response = await client.PostAsJsonAsync("/api/events", new CreateEventRequest
            {
                Name = "Should Not Be Created",
                EventDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(1)),
                Location = "Nowhere",
                Distance = 10.00m,
                EventType = "Run"
            });

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task CreateEvent_WithoutLogin_ReturnsUnauthorized()
        {
            var client = _factory.CreateClient();

            var response = await client.PostAsJsonAsync("/api/events", new CreateEventRequest
            {
                Name = "Should Not Be Created",
                EventDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(1)),
                Location = "Nowhere",
                Distance = 10.00m,
                EventType = "Run"
            });

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
// end of EventsControllerTests