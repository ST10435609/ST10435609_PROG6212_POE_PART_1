// start of HomeController
using System.Diagnostics;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Models;
using RaceDay.Mvc.Services;

namespace RaceDay.Mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApiClient _api_client;

        public HomeController(ApiClient api_client)
        {
            _api_client = api_client;
        }

        public async Task<IActionResult> Index(string? eventType)
        {
            var response = await _api_client.SendAsync(HttpMethod.Get, "api/events");
            var all_events = await response.Content.ReadFromJsonAsync<List<EventListItemViewModel>>() ?? new();

            var today = DateOnly.FromDateTime(DateTime.Today);
            var upcoming = all_events.Where(e => e.EventDate >= today);

            if (!string.IsNullOrEmpty(eventType))
            {
                upcoming = upcoming.Where(e => e.EventType == eventType);
            }

            ViewBag.SelectedType = eventType;
            return View(upcoming.OrderBy(e => e.EventDate).ToList());
        }

        public async Task<IActionResult> Details(int id)
        {
            var event_response = await _api_client.SendAsync(HttpMethod.Get, $"api/events/{id}");
            if (!event_response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var ev = await event_response.Content.ReadFromJsonAsync<EventDetailsViewModel>();

            var category_response = await _api_client.SendAsync(HttpMethod.Get, $"api/events/{id}/categories");
            ev!.Categories = await category_response.Content.ReadFromJsonAsync<List<CategoryViewModel>>() ?? new();

            return View(ev);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
// end of HomeController