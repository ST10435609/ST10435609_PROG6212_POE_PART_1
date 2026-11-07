// start of MvcControllerBase
using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Security;

namespace RaceDay.Mvc.Controllers
{
    public abstract class MvcControllerBase : Controller
    {
        protected int CurrentUserId => HttpContext.Session.GetInt32(MvcSessionKeys.UserId)!.Value;
        protected string CurrentRole => HttpContext.Session.GetString(MvcSessionKeys.Role)!;
    }
}
// end of MvcControllerBase