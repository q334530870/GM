using System;
using System.Linq;
using System.Web.Mvc;
using DKP.Core;
using DKP.Core.Infrastructure;

namespace DKP.Web.Framework.Filters
{
    /// <summary>
    /// 实现列表页面自动将Form数据装到ViewData中
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class FormFilter : ActionFilterAttribute
    {
        public FormFilter()
        {
            EnableFilteView = true;
            EnablePermissionValid = true;
        }

        /// <summary>
        /// form、querystring自动装载到ViewData中
        /// </summary>
        /// <param name="enableFilteView">是否启用过滤</param>
        public FormFilter(bool enableFilteView)
        {
            EnableFilteView = enableFilteView;
        }
        public FormFilter(bool enableFilteView, bool enablePermissionValid)
        {
            EnableFilteView = enableFilteView;
            EnablePermissionValid = enablePermissionValid;
        }

        public bool EnableFilteView { get; private set; }
        public bool EnablePermissionValid { get; private set; }

        public override void OnActionExecuting(ActionExecutingContext ctx)
        {
            if (ctx == null)
            {
                throw new ArgumentNullException("ctx");
            }
            var actionNames = new[] { "index", "select", "list" };
            var action = ctx.ActionDescriptor;
            var controller = action.ControllerDescriptor;

            if (EnablePermissionValid)
            {
                var session = EngineContext.Current.Resolve<IWorkContext>().CurrentUser;
                if (session == null) return;
                var permission =
                    session.Permissions.FirstOrDefault(
                        t => t.Action == action.ActionName && t.Controller == controller.ControllerName);

                if (EnableFilteView)
                {
                    if (permission == null) return;
                    if ((permission.Type != null && permission.Type != "List") ||
                        !actionNames.Contains(action.ActionName.ToLower())) return;
                }
            }

            foreach (string key in ctx.HttpContext.Request.Form)
            {
                ctx.Controller.ViewData[key] = ctx.HttpContext.Request.Form[key];
            }
            foreach (string key in ctx.HttpContext.Request.QueryString)
            {
                ctx.Controller.ViewData[key] = ctx.HttpContext.Request.QueryString[key];
            }

            var pageCurrent = ctx.HttpContext.Request.Form["pageCurrent"];
            var pageSize = ctx.HttpContext.Request.Form["pageSize"];
            if (string.IsNullOrEmpty(pageCurrent) || pageCurrent == "0") ctx.Controller.ViewData["pageCurrent"] = "1";
            if (string.IsNullOrEmpty(pageSize) || pageSize == "0") ctx.Controller.ViewData["pageSize"] = "20";
        }

    }
}