// start of SessionAuthorizeAttribute
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RaceDay.Api.Security
{
    // usage: [SessionAuthorize] = any logged-in user, [SessionAuthorize("Organiser")] = that role only
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SessionAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string? _required_role;

        public SessionAuthorizeAttribute(string? required_role = null)
        {
            _required_role = required_role;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var session = context.HttpContext.Session;
            var user_id = session.GetInt32(SessionKeys.UserId);

            // not logged in: 401
            if (user_id == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // logged in but wrong role: 403
            if (_required_role != null && session.GetString(SessionKeys.Role) != _required_role)
            {
                context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
            }
        }
    }
}
// end of SessionAuthorizeAttribute