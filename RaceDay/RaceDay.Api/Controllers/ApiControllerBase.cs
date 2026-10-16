// start of ApiControllerBase
using Microsoft.AspNetCore.Mvc;
using RaceDay.Api.Security;

namespace RaceDay.Api.Controllers
{
    // other controllers inherit from this instead of ControllerBase directly
    public abstract class ApiControllerBase : ControllerBase
    {
        // safe to read once [SessionAuthorize] has already confirmed the user is logged in
        protected int CurrentUserId => HttpContext.Session.GetInt32(SessionKeys.UserId)!.Value;

        protected string CurrentRole => HttpContext.Session.GetString(SessionKeys.Role)!;
       
        protected bool IsOwnerOf(RaceDay.Api.Models.Event ev) => ev.OrganiserId == CurrentUserId;
    }
}
// end of ApiControllerBase