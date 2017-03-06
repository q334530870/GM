using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using AutoMapper;
using log4net.Config;
using DKP.Web;
using DKP.Core.Infrastructure;
using DKP.Services.Task;
using System.Timers;
using DKP.Services.Security;
using DKP.Data;
using DKP.Core;
using log4net;
using System.Reflection;

namespace DKP.Web
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : HttpApplication
    {
        public static bool autoDkpWeek = true;
        public static ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected void Application_Start()
        {
            //log4net
            XmlConfigurator.Configure();

            //initialize engine context
            EngineContext.Initialize(false);

            //start scheduled tasks
            TaskManager.Instance.Initialize();
            TaskManager.Instance.Start();

            AreaRegistration.RegisterAllAreas();

            //定时任务，每周日为所有用户增加1点DKP
            //System.Timers.Timer objTimer = new Timer();
            //objTimer.Interval = 600000; //这个时间单位毫秒,比如10秒，就写10000 
            //objTimer.Enabled = true;
            //objTimer.Elapsed += new ElapsedEventHandler(TimeEvent);

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        private static void TimeEvent(object source, ElapsedEventArgs e)
        {
           
        }
    }
}

