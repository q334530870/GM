using System.Reflection;
using System.Text;
using System.Web.Mvc;
using log4net;

namespace DKP.Web.Framework.Filters
{

    /// <summary>
    /// 异常捕捉Filter，错误信息显示给客户端，并且把statuscode设为300。
    /// </summary>
    public class ExceptionFilter : FilterAttribute, IExceptionFilter
    {
        public ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void OnException(ExceptionContext filterContext)
        {
            //如果不是MessageException，就记录日志。
            if (!(filterContext.Exception is MessageException))
            {
                Logger.Error(filterContext.HttpContext.Request.RawUrl,
                             filterContext.Exception);
            }

            var jsonData = new
            {
                code = "300",
                msg = filterContext.Exception.Message + "\n" + filterContext.Exception.InnerException
            };

            filterContext.Result = new JsonResult()
            {
                Data = jsonData,
                ContentEncoding = Encoding.UTF8,
                ContentType = "text/html",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
            //filterContext.Result = new ContentResult()
            //{
            //    Content = "<script>$('#modal').modal('hide');window.parent.showMsg('error','"+ filterContext.Exception.InnerException + "','"+ filterContext.Exception.Message + "');</script>",
            //    ContentEncoding = Encoding.UTF8,
            //    ContentType = "text/html"
            //};
            filterContext.HttpContext.Response.Clear();
            filterContext.ExceptionHandled = true;
        }
    }
}