using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using DKP.Core;
using DKP.Core.Infrastructure;
using DKP.Core.Models;

namespace DKP.Web.Framework.Extensions
{
    public static class BtnColor
    {
        public static readonly string Default = "default";
        public static readonly string Close = "close";
        public static readonly string Red = "red";
        public static readonly string Blue = "blue";
        public static readonly string Green = "green";
        public static readonly string Link = "link";
        public static readonly string Orange = "orange";
    }

    public static class ToolbarExtensions
    {
        /// <summary>
        /// 根据当前用户权限情况，显示按钮，打开一个窗口
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="action">窗口Url</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="btnColor">按钮颜色</param>
        /// <param name="urlParameters">参数</param>
        /// <returns></returns>
        public static MvcHtmlString DialogButton(this HtmlHelper helper, string action, int width, int height, string btnColor = "default", object urlParameters = null)
        {
            var permissions = helper.ViewData["Current_Controller_Permissions"] as List<Permission>;
            if (permissions == null)
                return new MvcHtmlString(string.Empty);

            var permission = permissions.SingleOrDefault(p => p.Action == action);
            if (permission == null)
                return new MvcHtmlString(string.Empty);

            return helper.DialogButton(action, width, height, permission.Controller, permission.Area, permission.Name, permission.Icon, btnColor, urlParameters);
        }

        public static MvcHtmlString DialogButton(this HtmlHelper helper, string action, int width, int height, string name, string icon, string btnColor = "default", object urlParameters = null)
        {
            var permissions = helper.ViewData["Current_Controller_Permissions"] as List<Permission>;
            if (permissions == null)
                return new MvcHtmlString(string.Empty);

            var permission = permissions.SingleOrDefault(p => p.Action == action);
            if (permission == null)
                return new MvcHtmlString(string.Empty);

            return helper.DialogButton(action, width, height, permission.Controller, permission.Area, name, icon, btnColor, urlParameters);
        }

        public static MvcHtmlString DialogButton(this HtmlHelper helper, string action, int width, int height, string controller, string area, string name, string icon, string btnColor = "default", object urlParameters = null)
        {
            var session = EngineContext.Current.Resolve<IWorkContext>().CurrentUser;
            var permissions = session.Permissions;
            if (permissions == null)
                return new MvcHtmlString(string.Empty);

            var permission = permissions.SingleOrDefault(p => p.Action == action && p.Controller == controller && p.Area == area);

            //当前用户没有此操作权限
            if (permission == null) { return new MvcHtmlString(string.Empty); }

            //网站虚拟目录
            var appPath = helper.ViewContext.HttpContext.Request.ApplicationPath;
            if (appPath != "/") { appPath = appPath + "/"; }

            var url = appPath + permission.Area + "/" + permission.Controller + "/" + permission.Action;
            if (urlParameters != null)
            {
                var per = (new RouteValueDictionary(urlParameters)).Aggregate(string.Empty, (current, routeValue) => current + ("&" + routeValue.Key + "=" + routeValue.Value));
                if (!url.Contains("?")) { url += "?" + per.Remove(0, 1); }
            }

            var result = string.Format("<a href='{0}' data-id='{1}' data-width='{2}' data-height='{3}' data-icon='{4}' class='btn btn-{5}' data-toggle='dialog' data-mask='true' data-title='{6}'>{7}</a>",
                                       url, permission.Code, width, height,
                                       string.IsNullOrEmpty(icon) ? permission.Icon : icon,
                                       btnColor, permission.Title,
                                       string.IsNullOrEmpty(name) ? permission.Name : name);

            return new MvcHtmlString(result);
        }

        /// <summary>
        /// 根据当前用户权限情况，显示按钮，打开一个选项卡
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="action">选项卡Url</param>
        /// <param name="btnColor">按钮颜色</param>
        /// <param name="urlParameters">参数</param>
        /// <returns></returns>
        public static MvcHtmlString NatTabButton(this HtmlHelper helper, string action, string btnColor = "default", object urlParameters = null)
        {
            var permissions = helper.ViewData["Current_Controller_Permissions"] as List<Permission>;
            if (permissions == null)
                return new MvcHtmlString(string.Empty);

            var permission = permissions.SingleOrDefault(p => p.Action == action);

            //网站虚拟目录
            var appPath = helper.ViewContext.HttpContext.Request.ApplicationPath;
            if (appPath != "/") { appPath = appPath + "/"; }

            //当前用户没有此操作权限
            if (permission == null) { return new MvcHtmlString(string.Empty); }
            var url = appPath + permission.Area + "/" + permission.Controller + "/" + permission.Action;
            if (urlParameters != null)
            {
                var per = (new RouteValueDictionary(urlParameters)).Aggregate(string.Empty, (current, routeValue) => current + ("&" + routeValue.Key + "=" + routeValue.Value));
                if (!url.Contains("?")) { url += "?" + per.Remove(0, 1); }
            }

            var result = string.Format("<a href='{0}' data-id='{1}' data-icon='{2}' class='btn btn-{3}' data-toggle='navtab' data-title='{4}'>{5}</a>",
                                          url, permission.Code, permission.Icon, btnColor, permission.Title, permission.Name);

            return new MvcHtmlString(result);
        }

        public static MvcHtmlString NatTabButton(this HtmlHelper helper, string action, string controller, string area, string btnColor = "default", bool isButton = true, string defaultValue = "", string defaultText = "", object urlParameters = null)
        {
            var session = EngineContext.Current.Resolve<IWorkContext>().CurrentUser;
            var permissions = session.Permissions;
            if (permissions == null)
                return new MvcHtmlString(defaultValue);

            var permission = permissions.SingleOrDefault(p => p.Action == action
                && p.Controller == controller
                && p.Area == area);

            //网站虚拟目录
            var appPath = helper.ViewContext.HttpContext.Request.ApplicationPath;
            if (appPath != "/") { appPath = appPath + "/"; }

            //当前用户没有此操作权限
            if (permission == null) { return new MvcHtmlString(defaultValue); }
            var url = appPath + permission.Area + "/" + permission.Controller + "/" + permission.Action;
            if (urlParameters != null)
            {
                var per = (new RouteValueDictionary(urlParameters)).Aggregate(string.Empty, (current, routeValue) => current + ("&" + routeValue.Key + "=" + routeValue.Value));
                if (!url.Contains("?")) { url += "?" + per.Remove(0, 1); }
            }

            var result = string.Format("<a href='{0}' data-id='{1}' data-icon='{2}' class='{3}' data-toggle='navtab' data-title='{4}'>{5}</a>",
                                          url, permission.Code, permission.Icon,
                                          isButton ? "btn btn-" + btnColor : "",
                                          permission.Title,
                                          string.IsNullOrEmpty(defaultText) ? permission.Name : defaultText);

            return new MvcHtmlString(result);
        }

        public static MvcHtmlString ConfirmButton(this HtmlHelper helper, string action, string btnColor = "default", object urlParameters = null)
        {
            var permissions = helper.ViewData["Current_Controller_Permissions"] as List<Permission>;
            if (permissions == null)
                return new MvcHtmlString(string.Empty);

            var permission = permissions.SingleOrDefault(p => p.Action == action);

            //网站虚拟目录
            var appPath = helper.ViewContext.HttpContext.Request.ApplicationPath;
            if (appPath != "/") { appPath = appPath + "/"; }

            //当前用户没有此操作权限
            if (permission == null) { return new MvcHtmlString(string.Empty); }
            var url = appPath + permission.Area + "/" + permission.Controller + "/" + permission.Action;
            if (urlParameters != null)
            {
                var per = (new RouteValueDictionary(urlParameters)).Aggregate(string.Empty, (current, routeValue) => current + ("&" + routeValue.Key + "=" + routeValue.Value));
                if (!url.Contains("?")) { url += "?" + per.Remove(0, 1); }
            }

            var result = string.Format("<a href='{0}' data-toggle='doajax' data-confirm-msg='{1}' class='btn btn-{2}'>{3}</a>",
                                       url, permission.Title, btnColor, permission.Name);

            return new MvcHtmlString(result);
        }

        public static MvcHtmlString ConfirmButton(this HtmlHelper helper, string action, string controller, string area, string btnColor = "default", string callback = "", object urlParameters = null)
        {
            var session = EngineContext.Current.Resolve<IWorkContext>().CurrentUser;
            var permissions = session.Permissions;
            if (permissions == null)
                return new MvcHtmlString(string.Empty);

            var permission = permissions.SingleOrDefault(p => p.Action == action && p.Controller == controller && p.Area == area);

            //当前用户没有此操作权限
            if (permission == null) { return new MvcHtmlString(string.Empty); }

            //网站虚拟目录
            var appPath = helper.ViewContext.HttpContext.Request.ApplicationPath;
            if (appPath != "/") { appPath = appPath + "/"; }

            var url = appPath + permission.Area + "/" + permission.Controller + "/" + permission.Action;
            if (urlParameters != null)
            {
                var per = (new RouteValueDictionary(urlParameters)).Aggregate(string.Empty, (current, routeValue) => current + ("&" + routeValue.Key + "=" + routeValue.Value));
                if (!url.Contains("?")) { url += "?" + per.Remove(0, 1); }
            }

            var result = string.Format("<a href='{0}' data-toggle='doajax' data-confirm-msg='{1}' class='btn btn-{2}' data-callback='{3}'>{4}</a>",
                                       url, permission.Title, btnColor, callback, permission.Name);

            return new MvcHtmlString(result);
        }

        public static MvcHtmlString BatchConfirmButton(this HtmlHelper helper, string action, string idname, string group, string btnColor = "default", object urlParameters = null)
        {
            var permissions = helper.ViewData["Current_Controller_Permissions"] as List<Permission>;
            if (permissions == null)
                return new MvcHtmlString(string.Empty);

            var permission = permissions.SingleOrDefault(p => p.Action == action);

            //网站虚拟目录
            var appPath = helper.ViewContext.HttpContext.Request.ApplicationPath;
            if (appPath != "/") { appPath = appPath + "/"; }

            //当前用户没有此操作权限
            if (permission == null) { return new MvcHtmlString(string.Empty); }
            var url = appPath + permission.Area + "/" + permission.Controller + "/" + permission.Action;
            if (urlParameters != null)
            {
                var per = (new RouteValueDictionary(urlParameters)).Aggregate(string.Empty, (current, routeValue) => current + ("&" + routeValue.Key + "=" + routeValue.Value));
                if (!url.Contains("?")) { url += "?" + per.Remove(0, 1); }
            }

            var result = string.Format("<a href='{0}' data-toggle='doajaxchecked' data-confirm-msg='{1}' class='btn btn-{2}' data-idname='{4}' data-group='{5}'>{3}</a>",
                                       url, permission.Title, btnColor, permission.Name, idname, group);

            return new MvcHtmlString(result);
        }

        public static MvcHtmlString DownloadButton(this HtmlHelper helper, string action, string btnColor = "default", object urlParameters = null)
        {
            var permissions = helper.ViewData["Current_Controller_Permissions"] as List<Permission>;
            if (permissions == null)
                return new MvcHtmlString(string.Empty);

            var permission = permissions.SingleOrDefault(p => p.Action == action);

            //网站虚拟目录
            var appPath = helper.ViewContext.HttpContext.Request.ApplicationPath;
            if (appPath != "/") { appPath = appPath + "/"; }

            //当前用户没有此操作权限
            if (permission == null) { return new MvcHtmlString(string.Empty); }
            var url = appPath + permission.Area + "/" + permission.Controller + "/" + permission.Action;
            if (urlParameters != null)
            {
                var per = (new RouteValueDictionary(urlParameters)).Aggregate(string.Empty, (current, routeValue) => current + ("&" + routeValue.Key + "=" + routeValue.Value));
                if (!url.Contains("?")) { url += "?" + per.Remove(0, 1); }
            }

            var result = string.Format("<a href='{0}' data-toggle='doexport' data-confirm-msg='{1}' class='btn btn-{2}'>{3}</a>",
                                       url, permission.Title, btnColor, permission.Name);

            return new MvcHtmlString(result);
        }

        public static MvcHtmlString DownloadButton(this HtmlHelper helper, string action, string controller, string area, string btnColor = "default", object urlParameters = null)
        {
            var session = EngineContext.Current.Resolve<IWorkContext>().CurrentUser;
            var permissions = session.Permissions;
            if (permissions == null)
                return new MvcHtmlString(string.Empty);

            var permission = permissions.SingleOrDefault(p => p.Action == action && p.Controller == controller && p.Area == area);

            //网站虚拟目录
            var appPath = helper.ViewContext.HttpContext.Request.ApplicationPath;
            if (appPath != "/") { appPath = appPath + "/"; }

            //当前用户没有此操作权限
            if (permission == null) { return new MvcHtmlString(string.Empty); }
            var url = appPath + permission.Area + "/" + permission.Controller + "/" + permission.Action;
            if (urlParameters != null)
            {
                var per = (new RouteValueDictionary(urlParameters)).Aggregate(string.Empty, (current, routeValue) => current + ("&" + routeValue.Key + "=" + routeValue.Value));
                if (!url.Contains("?")) { url += "?" + per.Remove(0, 1); }
            }

            var result = string.Format("<a href='{0}' data-toggle='doexport' data-confirm-msg='{1}' class='btn btn-{2}'>{3}</a>",
                                       url, permission.Title, btnColor, permission.Name);

            return new MvcHtmlString(result);
        }

        public static bool CanOperate(this HtmlHelper helper, string action)
        {
            var permissions = helper.ViewData["Current_Controller_Permissions"] as List<Permission>;
            if (permissions == null)
                return false;

            var permission = permissions.SingleOrDefault(p => p.Action == action);

            //当前用户没有此操作权限
            if (permission == null)
            {
                return false;
            }
            return true;
        }
    }
}
