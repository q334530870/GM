using System.Web.Mvc;
using System.Web.Routing;

namespace DKP.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //// 首页
            //routes.MapRoute(
            //    "Home",
            //    "{action}.html",
            //    new { controller = "Home", action = "Index" },
            //    new[] { "DKP.Web.Controllers" });

            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                new[] { "DKP.Web.Controllers" });
        }
    }
}