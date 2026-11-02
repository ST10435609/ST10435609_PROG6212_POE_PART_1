// start of CategoriesController (MVC)
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Models;
using RaceDay.Mvc.Security;
using RaceDay.Mvc.Services;

namespace RaceDay.Mvc.Controllers
{
    [MvcSessionAuthorize("Organiser")]
    public class CategoriesController : MvcControllerBase
    {
        private readonly ApiClient _api_client;

        public CategoriesController(ApiClient api_client)
        {
            _api_client = api_client;
        }

        public async Task<IActionResult> Manage(int eventId)
        {
            var response = await _api_client.SendAsync(HttpMethod.Get, $"api/events/{eventId}/categories");
            var categories = await response.Content.ReadFromJsonAsync<List<CategoryViewModel>>() ?? new();
            ViewBag.EventId = eventId;
            return View(categories);
        }

        [HttpGet]
        public IActionResult Create(int eventId)
        {
            return View("Form", new CategoryFormViewModel { EventId = eventId });
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Form", model);
            }

            var body = new { model.CategoryName };
            var response = await _api_client.SendAsync(HttpMethod.Post, $"api/events/{model.EventId}/categories", body);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Could not add the category.");
                return View("Form", model);
            }

            return RedirectToAction(nameof(Manage), new { eventId = model.EventId });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, int eventId)
        {
            await _api_client.SendAsync(HttpMethod.Delete, $"api/categories/{id}");
            return RedirectToAction(nameof(Manage), new { eventId });
        }
    }
}
// end of CategoriesController (MVC)