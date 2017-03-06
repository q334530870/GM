using DKP.Core;
using DKP.Core.Models;
using DKP.Data;
using DKP.Data.Models;
using DKP.Services;
using DKP.Services.GM;
using DKP.Web.Framework.Controllers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DKP.Web.Controllers
{
    public class MailController : BaseController
    {
        private readonly IMailService _mailService;
        private readonly IServerService _serverService;
        public MailController(IMailService mailService, IServerService serverService)
        {
            _mailService = mailService;
            _serverService = serverService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List(string search, NameValueCollection forms, PageParam pageParam)
        {
            var mails = _mailService.getAll(Request.QueryString, pageParam);
            var results = new List<object>();
            //数据格式化
            foreach (var n in mails)
            {
                var items = "";
                foreach (var mi in n.MailItems)
                {
                    items += mi.TypeName + "-" + mi.ItemName + "-" + mi.Count + ";";
                }
                results.Add(new
                {
                    IdOp = n.Id,
                    Id = n.Id,
                    AgentName = n.AgentName,
                    ServerName = n.ServerName,
                    CharacterId = n.CharacterId,
                    CreateTime = n.CreateTime.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                    Title = n.Title,
                    Items = items,
                    Contents = CommonService.striphtml(n.Contents),
                    Status = n.Status == 1 ? "<span class='label label-primary'>已发送</span>" : "<span class='label label-default'>未发送</span>"
                });
            }
            var json = new
            {
                total = mails.total,
                rows = results
            };
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Add()
        {
            ViewData.Model = new Mail();
            return View();
        }

        [HttpPost]
        public ActionResult Add(Mail obj, List<string> Servers, List<MailItem> mi)
        {
            if (mi == null || mi.Count() == 0)
            {
                return Error("请添加物品！");
            }
            obj.MailItems = mi;
            _mailService.Add(obj);
            string msg = "添加成功！";
            var jsonModel = _mailService.SendMail(obj.Id);
            return Success(jsonModel.status?msg:(msg+"发送失败："+jsonModel.msg));
        }

        public ActionResult Delete(int id)
        {
            var obj = _mailService.GetById(id);
            if(obj.Status == 1)
            {
                return Error("已发送，无法删除！");
            }
            _mailService.Delete(id);
            return Success("删除成功！");
        }

        public ActionResult Send(int id)
        {
            var obj = _mailService.GetById(id);
            if (obj.Status == 1)
            {
                return Error("请勿重复发送！");
            }
            var jsonModel = _mailService.SendMail(id);
            if (jsonModel.status)
            {
                return Success("发送成功");
            }
            else
            {
                return Error("发送失败：" + jsonModel.msg);
            }
        }

        public ActionResult GetAgents()
        {
            var rst = _mailService.AgentList();
            if (rst.status)
            {
                var json = new
                {
                    value = rst.rows
                };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { value = "" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetTypes()
        {
            var rst = _mailService.TypeList();
            if (rst.status)
            {
                var json = new
                {
                    value = rst.rows
                };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { value=""}, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetServers(string id)
        {
            var rst = _mailService.ServerList(id);
            if (rst.status)
            {
                var json = new
                {
                    value = rst.rows
                };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { value = "" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetItems(string id)
        {
            var rst = _mailService.ItemList(id);
            if (rst.status)
            {
                var json = new
                {
                    value = rst.rows
                };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { value = "" }, JsonRequestBehavior.AllowGet);
            }
        }


    }
}
