// start of EventsController (MVC)
using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Models;
using RaceDay.Mvc.Security;
using RaceDay.Mvc.Services;

namespace RaceDay.Mvc.Controllers
{
    [MvcSessionAuthorize("Organiser")]
    public class EventsController : MvcControllerBase
    {
        private readonly ApiClient _api_client;

        public EventsController(ApiClient api_client)
        {
            _api_client = api_client;
        }

        public async Task<IActionResult> Manage()
        {
            var response = await _api_client.SendAsync(HttpMethod.Get, "api/events");
            var all_events = await response.Content.ReadFromJsonAsync<List<EventListItemViewModel>>() ?? new();

            var my_events = all_events.Where(e => e.OrganiserId == CurrentUserId).ToList();
            return View(my_events);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View("Form", new EventFormViewModel());
        }


        //here
        [HttpPost]
        public async Task<IActionResult> Create(EventFormViewModel model, IFormFile? bannerFile)
        {
            if (!ModelState.IsValid)
            {
                return View("Form", model);
            }

            var body = new
            {
                model.Name,
                model.Description,
                model.EventDate,
                model.Location,
                model.Distance,
                model.EventType
            };

            var response = await _api_client.SendAsync(HttpMethod.Post, "api/events", body);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Could not create the event. Check the event type is Run, Walk or Cycle.");
                return View("Form", model);
            }

            var created = await response.Content.ReadFromJsonAsync<EventFormViewModel>();

            if (bannerFile != null && bannerFile.Length > 0)
            {
                await _api_client.SendFileAsync(HttpMethod.Post, $"api/events/{created!.EventId}/banner", bannerFile);
            }

            TempData["SuccessMessage"] = "Event created.";
            return RedirectToAction(nameof(Manage));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _api_client.SendAsync(HttpMethod.Get, $"api/events/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var ev = await response.Content.ReadFromJsonAsync<EventFormViewModel>();
            return View("Form", ev);
        }
        //here
        [HttpPost]
        public async Task<IActionResult> Edit(EventFormViewModel model, IFormFile? bannerFile)
        {
            if (!ModelState.IsValid)
            {
                return View("Form", model);
            }

            var body = new
            {
                model.Name,
                model.Description,
                model.EventDate,
                model.Location,
                model.Distance,
                model.EventType
            };

            var response = await _api_client.SendAsync(HttpMethod.Put, $"api/events/{model.EventId}", body);

            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Could not update the event.");
                return View("Form", model);
            }

            if (bannerFile != null && bannerFile.Length > 0)
            {
                await _api_client.SendFileAsync(HttpMethod.Post, $"api/events/{model.EventId}/banner", bannerFile);
            }

            TempData["SuccessMessage"] = "Event updated.";
            return RedirectToAction(nameof(Manage));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _api_client.SendAsync(HttpMethod.Delete, $"api/events/{id}");

            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                TempData["ErrorMessage"] = "Cannot delete this event — it still has categories or enrolments.";
            }
            else if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Could not delete the event.";
            }
            else
            {
                TempData["SuccessMessage"] = "Event deleted.";
            }

            return RedirectToAction(nameof(Manage));
        }

        public async Task<IActionResult> Enrolments(int id)
        {
            var response = await _api_client.SendAsync(HttpMethod.Get, $"api/events/{id}/enrolments");

            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            var enrolments = await response.Content.ReadFromJsonAsync<List<EnrolmentListItemViewModel>>() ?? new();
            ViewBag.EventId = id;
            return View(enrolments);
        }

        [HttpGet]
        public IActionResult CaptureResult(int enrolmentId)
        {
            return View(new ResultFormViewModel { EnrolmentId = enrolmentId });
        }

        [HttpPost]
        public async Task<IActionResult> CaptureResult(ResultFormViewModel model, int eventId)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var body = new { model.FinishTime, model.FinishingPosition };
            var response = await _api_client.SendAsync(HttpMethod.Post, $"api/enrolments/{model.EnrolmentId}/result", body);

            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                ModelState.AddModelError(string.Empty, "A result has already been captured for this enrolment.");
                return View(model);
            }

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Could not capture the result.");
                return View(model);
            }

            TempData["SuccessMessage"] = "Result captured.";
            return RedirectToAction(nameof(Enrolments), new { id = eventId });
        }
    }
}
// end of EventsController (MVC)