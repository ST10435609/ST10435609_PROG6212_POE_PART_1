// start of ApiClient
using System.Net.Http.Json;

namespace RaceDay.Mvc.Services
{
    // every call the MVC app makes to the API goes through here
    public class ApiClient
    {
        private const string SessionCookieKey = "ApiSessionCookie";

        private readonly IHttpClientFactory _http_client_factory;
        private readonly IHttpContextAccessor _http_context_accessor;

        public ApiClient(IHttpClientFactory http_client_factory, IHttpContextAccessor http_context_accessor)
        {
            _http_client_factory = http_client_factory;
            _http_context_accessor = http_context_accessor;
        }

        public async Task<HttpResponseMessage> SendAsync(HttpMethod method, string path, object? body = null)
        {
            var client = _http_client_factory.CreateClient("RaceDayApi");
            var request = new HttpRequestMessage(method, path);

            // attach the API's session cookie saved earlier at login
            var session = _http_context_accessor.HttpContext!.Session;
            var saved_cookie = session.GetString(SessionCookieKey);

            if (!string.IsNullOrEmpty(saved_cookie))
            {
                request.Headers.Add("Cookie", saved_cookie);
            }

            if (body != null)
            {
                request.Content = JsonContent.Create(body);
            }

            var response = await client.SendAsync(request);

            // if the API set a new session cookie on this response (e.g. on login), save it
            if (response.Headers.TryGetValues("Set-Cookie", out var set_cookie_values))
            {
                foreach (var raw in set_cookie_values)
                {
                    
                    var name_value_part = raw.Split(';')[0];

                    if (name_value_part.StartsWith(".AspNetCore.Session"))
                    {
                        session.SetString(SessionCookieKey, name_value_part);
                    }
                }
            }

            return response;
        }

        public void ClearApiSessionCookie()
        {
            _http_context_accessor.HttpContext!.Session.Remove(SessionCookieKey);
        }
    }
}
// end of ApiClient