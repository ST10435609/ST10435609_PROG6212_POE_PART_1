// start of MvcSessionAuthorizeAttribute
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RaceDay.Mvc.Security
{
    // usage: [MvcSessionAuthorize] = any logged-in user, [MvcSessionAuthorize("Organiser")] = that role only
    public class MvcSessionAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string? _required_role;

        public MvcSessionAuthorizeAttribute(string? required_role = null)
        {
            _required_role = required_role;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var session = context.HttpContext.Session;
            var user_id = session.GetInt32(MvcSessionKeys.UserId);

            if (user_id == null)
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            if (_required_role != null && session.GetString(MvcSessionKeys.Role) != _required_role)
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Home", null);
            }
        }
    }
}
// end of MvcSessionAuthorizeAttribute