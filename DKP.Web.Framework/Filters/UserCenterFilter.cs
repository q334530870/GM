using System.Web.Mvc;
using DKP.Core;
using DKP.Core.Infrastructure;

namespace DKP.Web.Framework.Filters
{
    /// <summary>
    /// 判断用户是否已经登录，如果没有登录或者登录已经过期就跳转到登录页面重新登录。
    /// </summary>
    public class UserCenterFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext ctx)
        {
            var loginUrl = new UrlHelper(ctx.RequestContext).Action("Login", "Account", new { area = "" });

            if (ctx.HttpContext.Session == null)
            {
                ctx.Result = new ContentResult()
                {
                    Content = "<script language='javascript'>window.location.href = '" + loginUrl + "';</script>"
                };
                ctx.HttpContext.Response.Clear();
            }
            else
            {
                if (ctx.HttpContext.Session == null) return;
                var workContext = EngineContext.Current.Resolve<IWorkContext>();

                if (workContext.IsLogin == false)
                {
                    ctx.Result = new ContentResult()
                    {
                        Content = "<script language='javascript'>window.location.href = '" + loginUrl + "';</script>"
                    };
                    ctx.HttpContext.Response.Clear();
                    return;
                }
            }
        }
    }
}