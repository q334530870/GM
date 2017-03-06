using System.Web.Mvc;
using DKP.Core;
using DKP.Core.Infrastructure;
using System.Collections.Generic;

namespace DKP.Web.Framework.Filters
{
    /// <summary>
    /// 判断用户是否已经登录，如果没有登录或者登录已经过期就跳转到登录页面重新登录。
    /// </summary>
    public class AuthFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext ctx)
        {
            ActionDescriptor action = ctx.ActionDescriptor;
            ControllerDescriptor controller = action.ControllerDescriptor;

            if (action.ActionName.EndsWith("Download") || action.ActionName == "Archive")
            {
                return;
            }

            if (action.ActionName.ToLower() == "list")
            {
                return;
            }

            //如果是以"_"开头的Action，就跳过验证。
            if (action.ActionName.StartsWith("_"))
            {
                return;
            }

            if (ctx.HttpContext.Session != null)
            {
                var workContext = EngineContext.Current.Resolve<IWorkContext>();

                //检查是否登录
                if (workContext.IsLogin == false)
                {
                    var loginUrl = new UrlHelper(ctx.RequestContext).Action("Login", "Account", new { area = "" });
                    ctx.Result = new ContentResult()
                    {
                        Content = "<script language='javascript'>" +
                                  "try {" +
                                  "alertMsg.error('会话超时！请重新登录！', {okCall:function(){window.location.href = '" + loginUrl + "';}});" +
                                  "}" +
                                  "catch(e) {" +
                                  "window.location.href = '" + loginUrl + "';}" +
                                  "</script>"
                    };
                    ctx.HttpContext.Response.Clear();
                    return;
                }

                //检查是否能登录后台管理
                if (!workContext.CurrentUser.IsAllowEnterAdmin)
                {
                    var loginUrl = new UrlHelper(ctx.RequestContext).Action("Login", "Account", new { area = "" });
                    ctx.Result = new ContentResult()
                    {
                        Content = "<script language='javascript'>window.location.href = '" + loginUrl + "';</script>"
                    };
                    ctx.HttpContext.Response.Clear();
                    return;
                }

                //检测是否有权限执行当前controller当前action
                if (workContext.CurrentUser.IsSystemAdmin) //超级用户不需要判断
                {
                    return;
                }
                List<Core.Models.Permission> perms = workContext.CurrentUser.Permissions;
                var perm = new Core.Models.Permission();
                foreach(var p in perms)
                {
                    if((p.Controller == controller.ControllerName) && (p.Action == action.ActionName || "Do" + p.Action == action.ActionName))
                    {
                        perm = p;
                    }
                }
                if (perm.Id == 0)
                {
                    ctx.Result = new ContentResult()
                    {
                        Content = "<script language='javascript'>window.parent.showMsg('error', '对不起，你没用此操作权限！');</script>"
                    };
                    ctx.HttpContext.Response.Clear();
                }
            }
            else
            {
                var loginUrl = new UrlHelper(ctx.RequestContext).Action("Login", "Account", new { area = "" });
                ctx.Result = new ContentResult()
                {
                    Content = "<script language='javascript'>" +
                              "try {" +
                              "alertMsg.error('会话超时！请重新登录！', {okCall:function(){window.location.href = '" + loginUrl + "';}});" +
                              "}" +
                              "catch(e) {" +
                              "window.location.href = '" + loginUrl + "';}" +
                              "</script>"
                };
                ctx.HttpContext.Response.Clear();
                return;
            }
        }
    }
}