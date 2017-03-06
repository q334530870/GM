using DKP.Core.Models;
using DKP.Data.Models;
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
    public class MemberController : BaseController
    {
        private readonly IMemberService _memberService;
        public MemberController(IMemberService memberService)
        {
            _memberService = memberService;
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List(string search, PageParam pageParam)
        {
            var result = _memberService.MemberList(Request.QueryString,pageParam.offset, pageParam.limit);
            var members = new List<Member>();
            foreach (var r in result.rows)
            {
                var status = r["status"].ToString();
                var ms = MemberStatus.getMS(status);
                members.Add(new Member {
                    id = r["id"] + "",
                    account_id = r["account_id"] + "",
                    last_login_time = r["last_login_time"] + "",
                    platform_uid = r["platform_uid"] + "",
                    user_id = r["user_id"] + "",
                    status = "<span class='label label-"+ms.color+"'>"+ms.name+"</span>",
                    platform = r["platform"] + "",
                    server_id = r["server_id"] + "",
                    create_time = r["create_time"] + "",
                    device_id = r["device_id"] + "",
                    device_type = r["device_type"] + "",
                    platform_name = r["platform_name"] + "",
                    name = r["name"] + "",
                    opId = r["account_id"] + ","+ r["platform"]+","+ r["user_id"]+","+r["server_id"]
                });
            }
            var json = new
            {
                status = result.status,
                msg = result.msg,
                total = result.total,
                rows = members
            };
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Add()
        {
            ViewData.Model = new Member();
            return View();
        }
        [HttpPost]
        public ActionResult Add(string account_id,string platform, string password)
        {
            var jsonModel = _memberService.Add(account_id,platform,password);
            return Success(jsonModel.status ? "添加成功" : ("添加失败：" + jsonModel.msg));
        }

        public ActionResult Edit(string id)
        {
            var account_id = id.Split(',')[0];
            var platform = id.Split(',')[1];
            var result = _memberService.MemberList(account_id,platform);
            var member = new Member();
            foreach (var r in result.rows)
            {
                var account_id2 = r["account_id"].ToString();
                if(account_id2 == account_id)
                {
                    member = new Member
                    {
                        id = r["id"] + "",
                        account_id = r["account_id"] + "",
                        last_login_time = r["last_login_time"] + "",
                        platform_uid = r["platform_uid"] + "",
                        user_id = r["user_id"] + "",
                        status = r["status"] + "",
                        platform = r["platform"] + "",
                        server_id = r["server_id"] + "",
                        create_time = r["create_time"] + "",
                        device_id = r["device_id"] + "",
                        device_type = r["device_type"] + "",
                        platform_name = r["platform_name"] + "",
                        opId = r["user_id"] + ""
                    };
                }
            }
            if (member == null || string.IsNullOrEmpty(member.account_id))
            {
                return Error("用户不存在");
            }
            ViewData.Model = member;
            return View();
        }
        [HttpPost]
        public ActionResult Edit(string account_id, string platform, string password)
        {
            var jsonModel = _memberService.Update(account_id, platform, password);
            return Success(jsonModel.status ? "修改成功" : ("修改失败：" + jsonModel.msg));
        }

        public ActionResult JY(string id)
        {
            var jsonModel = _memberService.JY(id,"2");
            return Success(jsonModel.status ? "禁言成功" : ("禁言失败：" + jsonModel.msg));
        }

        public ActionResult SH(string id)
        {
            var jsonModel = _memberService.SH(id,"1");
            return Success(jsonModel.status ? "锁号成功" : ("锁号失败：" + jsonModel.msg));
        }

    }
}
