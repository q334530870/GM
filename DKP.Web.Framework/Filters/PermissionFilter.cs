using System.Linq;
using System.Web.Mvc;
using DKP.Core;
using DKP.Core.Infrastructure;
using DKP.Core.Models;

namespace DKP.Web.Framework.Filters
{
    /// <summary>
    /// 取当前Controller下，用户权限情况。
    /// </summary>
    public class PermissionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext ctx)
        {
            if (ctx.HttpContext.Session != null)
            {
                var session = EngineContext.Current.Resolve<IWorkContext>().CurrentUser;

                if (session != null)
                {
                    ActionDescriptor action = ctx.ActionDescriptor;
                    ControllerDescriptor controller = action.ControllerDescriptor;

                    var area = ctx.RouteData.DataTokens["area"] ?? "";

                    var permisssions = session.Permissions
                        .Where(p => p.Area == area.ToString() && p.Controller == controller.ControllerName && p.Level == 3)
                        .OrderBy(p => p.Order).ToList();

                    ctx.Controller.ViewData["Current_Controller_Permissions"] = permisssions;

                }
            }
        }
    }
}