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
    public class CodeController : BaseController
    {

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List(string search, NameValueCollection forms, PageParam pageParam)
        {
            var a = Request.QueryString;
            var mails = new List<object>();
            mails.Add(new { Number="100001", Name="新手礼包",Content="体力药水*1;金币包*1",Start="2016-10-11",End="2016-10-15", Count="6888/9999" });
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
