using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using DKP.Core.Infrastructure;

namespace DKP.Web.Framework.Security.Captcha
{
    public class CaptchaValidatorAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            bool valid = false;
            var captchaCode = filterContext.HttpContext.Request.Form["captcha_code"];
            if (!string.IsNullOrEmpty(captchaCode) && filterContext.HttpContext.Session != null)
            {
                var captchaSettings = EngineContext.Current.Resolve<CaptchaSettings>();
                if (captchaSettings.Enabled)
                {
                    valid = String.Equals(filterContext.HttpContext.Session["captcha_code"].ToString(), captchaCode, StringComparison.CurrentCultureIgnoreCase);
                }
            }

            //this will push the result value into a parameter in our Action  
            filterContext.ActionParameters["captchaValid"] = valid;

            base.OnActionExecuting(filterContext);
        }
    }
}
