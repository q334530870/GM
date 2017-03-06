using DKP.Core;
using DKP.Services.Security;
using DKP.Web.Framework.Filters;
using System.Collections.Generic;
using System.Web.Mvc;

namespace DKP.Web.Controllers
{
    [ExceptionFilter]
    [CompressFilter]
    public class HomeController : Controller
    {
        private readonly IUserService _userService;
        private readonly IWorkContext _workContext;

        public HomeController(
            IUserService userService,
            IWorkContext workContext)
        {
            _userService = userService;
            _workContext = workContext;
        }

        public ActionResult Index()
        {
            //更新权限
            _workContext.Reset(_workContext.CurrentUserId);
            var user = _workContext.CurrentUser;
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            ViewBag.Permissions = user.Permissions;
            ViewBag.UsereId = user.UserId;
            ViewBag.Name = user.Name;
            ViewBag.RealName = user.RealName;
            ViewBag.Avatar = user.Avatar;
            ViewBag.Language = _workContext.WorkingLanguage;
            ViewBag.HomeTitle = "GM管理后台";
            return View();
        }

        public ActionResult SecIndex()
        {
            return View();
        }

    }
}
