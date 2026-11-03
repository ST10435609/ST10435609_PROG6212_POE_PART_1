// start of ParticipantController
using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Models;
using RaceDay.Mvc.Security;
using RaceDay.Mvc.Services;

namespace RaceDay.Mvc.Controllers
{
    [MvcSessionAuthorize("Participant")]
    public class ParticipantController : MvcControllerBase
    {
        private readonly ApiClient _api_client;

        public ParticipantController(ApiClient api_client)
        {
            _api_client = api_client;
        }

        [HttpPost]
        public async Task<IActionResult> Enrol(int eventId, int categoryId)
        {
            var body = new { categoryId };
            var response = await _api_client.SendAsync(HttpMethod.Post, $"api/events/{eventId}/enrolments", body);

            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                TempData["ErrorMessage"] = "You're already enrolled in this event.";
            }
            else if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Could not enrol. Please try again.";
            }
            else
            {
                TempData["SuccessMessage"] = "You're enrolled!";
            }

            return RedirectToAction("Details", "Home", new { id = eventId });
        }

        public async Task<IActionResult> MyEnrolments()
        {
            var response = await _api_client.SendAsync(HttpMethod.Get, "api/users/me/enrolments");
            var enrolments = await response.Content.ReadFromJsonAsync<List<ParticipantEnrolmentViewModel>>() ?? new();
            return View(enrolments);
        }

        public async Task<IActionResult> MyResults()
        {
            var response = await _api_client.SendAsync(HttpMethod.Get, "api/users/me/results");
            var results = await response.Content.ReadFromJsonAsync<List<ParticipantResultViewModel>>() ?? new();
            return View(results);
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var response = await _api_client.SendAsync(HttpMethod.Get, "api/users/me");
            var profile = await response.Content.ReadFromJsonAsync<ProfileViewModel>();
            return View(profile);
        }

        [HttpPost]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var body = new { model.FullName, model.Email, model.ProfilePictureUrl };
            var response = await _api_client.SendAsync(HttpMethod.Put, "api/users/me", body);

            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                ModelState.AddModelError(string.Empty, "That email is already in use by another account.");
                return View(model);
            }

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Could not update your profile.");
                return View(model);
            }

            // keep the MVC session's display name in sync with the update
            HttpContext.Session.SetString(RaceDay.Mvc.Security.MvcSessionKeys.FullName, model.FullName);

            TempData["SuccessMessage"] = "Profile updated.";
            return View(model);
        }
    }
}
// end of ParticipantController