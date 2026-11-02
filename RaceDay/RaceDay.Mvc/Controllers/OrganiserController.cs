// start of OrganiserController
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Models;
using RaceDay.Mvc.Security;
using RaceDay.Mvc.Services;

namespace RaceDay.Mvc.Controllers
{
    [MvcSessionAuthorize("Organiser")]
    public class OrganiserController : MvcControllerBase
    {
        private readonly ApiClient _api_client;

        public OrganiserController(ApiClient api_client)
        {
            _api_client = api_client;
        }

        public async Task<IActionResult> Dashboard()
        {
            var response = await _api_client.SendAsync(HttpMethod.Get, "api/events");
            var all_events = await response.Content.ReadFromJsonAsync<List<EventListItemViewModel>>() ?? new();

            var my_events = all_events.Where(e => e.OrganiserId == CurrentUserId).ToList();

            foreach (var ev in my_events)
            {
                var enrol_response = await _api_client.SendAsync(HttpMethod.Get, $"api/events/{ev.EventId}/enrolments");
                if (enrol_response.IsSuccessStatusCode)
                {
                    var enrolments = await enrol_response.Content.ReadFromJsonAsync<List<EnrolmentListItemViewModel>>();
                    ev.EnrolmentCount = enrolments?.Count ?? 0;
                }
            }

            var model = new OrganiserDashboardViewModel
            {
                Events = my_events.OrderBy(e => e.EventDate).ToList()
            };

            return View(model);
        }
    }
}
// end of OrganiserController