// start of AccountController
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Models;
using RaceDay.Mvc.Security;
using RaceDay.Mvc.Services;

namespace RaceDay.Mvc.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApiClient _api_client;

        public AccountController(ApiClient api_client)
        {
            _api_client = api_client;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var response = await _api_client.SendAsync(HttpMethod.Post, "api/auth/register", model);

            if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                ModelState.AddModelError(string.Empty, "That email is already registered.");
                return View(model);
            }

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Something went wrong. Please try again.");
                return View(model);
            }

            TempData["SuccessMessage"] = "Account created. Please log in.";
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var response = await _api_client.SendAsync(HttpMethod.Post, "api/auth/login", model);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return View(model);
            }

            var auth = await response.Content.ReadFromJsonAsync<AuthResult>();

            // MVC app's OWN session, separate from the API's session
            // cookie the ApiClient already captured and stored internally
            HttpContext.Session.SetInt32(MvcSessionKeys.UserId, auth!.UserId);
            HttpContext.Session.SetString(MvcSessionKeys.FullName, auth.FullName);
            HttpContext.Session.SetString(MvcSessionKeys.Role, auth.Role);

            if (auth.Role == "Organiser")
            {
                return RedirectToAction("Dashboard", "Organiser");
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _api_client.SendAsync(HttpMethod.Post, "api/auth/logout");
            _api_client.ClearApiSessionCookie();
            HttpContext.Session.Clear();

            return RedirectToAction(nameof(Login));
        }

        // matches API's AuthResponse shape
        private class AuthResult
        {
            public int UserId { get; set; }
            public string FullName { get; set; } = string.Empty;
            public string Role { get; set; } = string.Empty;
        }
    }
}
// end of AccountController