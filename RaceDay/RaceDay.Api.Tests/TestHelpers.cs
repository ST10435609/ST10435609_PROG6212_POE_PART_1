// start of TestHelpers
using System.Net.Http.Json;
using RaceDay.Api.Dtos;

namespace RaceDay.Api.Tests
{
    public static class TestHelpers
    {
        // registers and logs a user in on the given client
        public static async Task RegisterAndLoginAsync(
            HttpClient client, string email, string password, string role)
        {
            await client.PostAsJsonAsync("/api/auth/register", new RegisterRequest
            {
                FullName = "Test User",
                Email = email,
                Password = password,
                Role = role
            });

            var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest
            {
                Email = email,
                Password = password
            });

            loginResponse.EnsureSuccessStatusCode();
        }

        // logs in as an Organiser and creates one event with one category,

        public static async Task<(int eventId, int categoryId)> CreateEventWithCategoryAsync(
            HttpClient organiserClient)
        {
            var eventResponse = await organiserClient.PostAsJsonAsync("/api/events", new CreateEventRequest
            {
                Name = "Joburg City 10K",
                EventDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(1)),
                Location = "Johannesburg",
                Distance = 10.00m,
                EventType = "Run"
            });

            eventResponse.EnsureSuccessStatusCode();
            var ev = await eventResponse.Content.ReadFromJsonAsync<EventResponse>();

            var categoryResponse = await organiserClient.PostAsJsonAsync(
                $"/api/events/{ev!.EventId}/categories",
                new CategoryRequest { CategoryName = "Senior" });

            categoryResponse.EnsureSuccessStatusCode();
            var category = await categoryResponse.Content.ReadFromJsonAsync<CategoryResponse>();

            return (ev.EventId, category!.CategoryId);
        }
    }
}
// end of TestHelpers