using DKP.Core.Models;
using DKP.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DKP.Web.Controllers
{
    public class FuncController : BaseController
    {

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List(string search, NameValueCollection forms, PageParam pageParam)
        {
            var a = Request.QueryString;
            var mails = new List<object>();
            mails.Add(new { Name = "防沉迷", Status = "<span class='label label-primary'>开启</span>" });
            mails.Add( new { Name = "防沉迷", Status = "<span class='label label-default'>关闭</span>" });
            var json = new
            {
                total = mails.Count(),
                rows = mails
            };
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Add()
        {
            return View();
        }

    }
}
