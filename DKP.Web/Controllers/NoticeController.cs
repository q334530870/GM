using DKP.Core.Models;
using DKP.Data;
using DKP.Data.Models;
using DKP.Services;
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
    public class NoticeController : BaseController
    {
        private readonly INoticeService _noticeService;
        private readonly IServerService _serverService;
        public NoticeController(INoticeService noticeService, IServerService serverService)
        {
            _noticeService = noticeService;
            _serverService = serverService;
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List(PageParam pageParam)
        {
            var notices = _noticeService.getAll(Request.QueryString,pageParam);
            var results = new List<object>();
            //数据格式化
            foreach(var n in notices)
            {
                results.Add(new {
                    IdOp = n.Id,
                    Id = n.Id,
                    ServerIds = n.ServerIds,
                    StartDate = n.StartDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    EndDate = n.EndDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    RepeatTime = n.RepeatTime + "分钟",
                    Contents = CommonService.striphtml(n.Contents),
                    Type = (n.Type == 1 ? "聊天频道" : "全频滚动"),
                    Status = n.Status == 1? "<span class='label label-primary'>已推送</span>" : "<span class='label label-default'>未推送</span>"
                });
            }
            var json = new
            {
                total = notices.total,
                rows = results
            };
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Add()
        {
            var json = _serverService.ServerList();
            ViewData.Model = new Notice();
            ViewBag.Servers = json.rows;
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Add(Notice notice,List<string> Servers)
        {
            if (Servers != null)
            {
                notice.ServerIds = string.Join(",", Servers);
            }
            _noticeService.Add(notice);
            //TODO:推送到游戏服务器
            return Success("添加成功！");
        }

        public ActionResult Edit(int id)
        {
            var json = _serverService.ServerList();
            ViewBag.Servers = json.rows;
            var notice = _noticeService.GetById(id);
            return View(notice);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(Notice notice, List<string> Servers)
        {
            if (Servers != null)
            {
                notice.ServerIds = string.Join(",", Servers);
            }
            _noticeService.Update(notice);
            //TODO:推送到游戏服务器
            return Success("修改成功！");
        }

        public ActionResult Delete(int id)
        {
            _noticeService.Delete(id);
            return Success("删除成功！");
        }

    }
}
