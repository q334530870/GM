using DKP.Core.Models;
using DKP.Data;
using DKP.Services;
using DKP.Services.Helpers;
using DKP.Services.GM;
using DKP.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DKP.Web.Controllers
{
    public class ServerController : BaseController
    {
        private readonly IServerService _serverService;

        public ServerController(IServerService serverService)
        {
            _serverService = serverService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List(string search, NameValueCollection forms, PageParam pageParam)
        {
            var json = _serverService.ServerList(0);
            return ListResult(json);
        }

        public ActionResult Auth()
        {
            var json = _serverService.ServerAuthList();
            ViewBag.Servers = json.rows;
            return View();
        }

    }
}
