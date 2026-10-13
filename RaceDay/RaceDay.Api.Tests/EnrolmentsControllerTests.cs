// start of EnrolmentsControllerTests
using System.Net;
using System.Net.Http.Json;
using RaceDay.Api.Dtos;
using Xunit;

namespace RaceDay.Api.Tests
{
    public class EnrolmentsControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;

        public EnrolmentsControllerTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Enrol_AsParticipant_ReturnsCreatedAndIsRecorded()
        {
            var organiserClient = _factory.CreateClient();
            await TestHelpers.RegisterAndLoginAsync(
                organiserClient, $"org_{Guid.NewGuid()}@test.com", "Password123", "Organiser");

            var (eventId, categoryId) = await TestHelpers.CreateEventWithCategoryAsync(organiserClient);

            var participantClient = _factory.CreateClient();
            await TestHelpers.RegisterAndLoginAsync(
                participantClient, $"part_{Guid.NewGuid()}@test.com", "Password123", "Participant");

            var enrolResponse = await participantClient.PostAsJsonAsync(
                $"/api/events/{eventId}/enrolments",
                new CreateEnrolmentRequest { CategoryId = categoryId });

            Assert.Equal(HttpStatusCode.Created, enrolResponse.StatusCode);

            // confirm it was actually recorded, not just that the call succeeded
            var myEnrolments = await participantClient.GetFromJsonAsync<List<EnrolmentResponse>>(
                "/api/users/me/enrolments");

            Assert.Contains(myEnrolments!, e => e.EventId == eventId && e.CategoryId == categoryId);
        }

        [Fact]
        public async Task Enrol_AsOrganiser_ReturnsForbidden()
        {
            var organiserClient = _factory.CreateClient();
            await TestHelpers.RegisterAndLoginAsync(
                organiserClient, $"org_{Guid.NewGuid()}@test.com", "Password123", "Organiser");

            var (eventId, categoryId) = await TestHelpers.CreateEventWithCategoryAsync(organiserClient);

            // the SAME organiser tries to enrol in their own event, using the Organiser role
            var response = await organiserClient.PostAsJsonAsync(
                $"/api/events/{eventId}/enrolments",
                new CreateEnrolmentRequest { CategoryId = categoryId });

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task Enrol_Twice_ReturnsConflict()
        {
            var organiserClient = _factory.CreateClient();
            await TestHelpers.RegisterAndLoginAsync(
                organiserClient, $"org_{Guid.NewGuid()}@test.com", "Password123", "Organiser");

            var (eventId, categoryId) = await TestHelpers.CreateEventWithCategoryAsync(organiserClient);

            var participantClient = _factory.CreateClient();
            await TestHelpers.RegisterAndLoginAsync(
                participantClient, $"part_{Guid.NewGuid()}@test.com", "Password123", "Participant");

            await participantClient.PostAsJsonAsync(
                $"/api/events/{eventId}/enrolments",
                new CreateEnrolmentRequest { CategoryId = categoryId });

            var secondAttempt = await participantClient.PostAsJsonAsync(
                $"/api/events/{eventId}/enrolments",
                new CreateEnrolmentRequest { CategoryId = categoryId });

            Assert.Equal(HttpStatusCode.Conflict, secondAttempt.StatusCode);
        }
    }
}
// end of EnrolmentsControllerTests