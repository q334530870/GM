using DKP.Core;
using DKP.Core.Models;
using DKP.Services.Security;
using DKP.Web.Framework.Controllers;
using DKP.Web.Framework.Filters;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DKP.Web.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IWorkContext _workContext;
        public UserController(
           IUserService userService,
           IRoleService roleService,
           IWorkContext workContext)
        {
            _userService = userService;
            _roleService = roleService;
            _workContext = workContext;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List(string search, NameValueCollection forms,PageParam pageParam)
        {
            var users = _userService.GetNoAdminUsers(forms, pageParam);
            foreach(var u in users)
            {
                u.Roles.Clear();
            }
            var json = new
            {
                total = users.total,
                rows = users
            };
            return Json(json,JsonRequestBehavior.AllowGet);
        }

        public ActionResult Add()
        {
            ViewBag.Roles = _roleService.GetAllRoles();
            ViewData.Model = new DKP.Data.User();
            return View();
        }

        public ActionResult Edit(int id)
        {
            var user = _userService.GetUser(id);
            ViewBag.Roles = _roleService.GetAllRoles();
            ViewData.Model = _userService.GetUser(id);
            return View();
        }

        [HttpPost]
        public ActionResult Add(DKP.Data.User user,int[] rid)
        {
            if (rid == null || rid.Count() <= 0)
            {
                return Json(new AjaxResult { code = 300, msg = "请选择角色！" });
            }
            _userService.AddUser(user, rid);
            return Json(new AjaxResult { code=200,msg="用户添加成功！"});
        }

        [HttpPost]
        public ActionResult Edit(DKP.Data.User user, int[] rid)
        {
            if (rid == null || rid.Count() <= 0)
            {
                return Json(new AjaxResult { code = 300, msg = "请选择角色！" });
            }
            user.Roles = _roleService.GetAllRoles().Where(r => r.Users.Where(u => u.Id == user.Id).Count() > 0).ToList();
            _userService.UpdateUser(user, rid);
            return Json(new AjaxResult { code = 200, msg = "用户修改成功！" });
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            _userService.DeleteUser(id);
            return Json(new AjaxResult { code = 200, msg = "用户删除成功！" });
        }

        public ActionResult EditPassword(int id)
        {
            ViewBag.Id = id;
            return View();
        }

        [HttpPost]
        public ActionResult EditPassword(int id,string OldPassword, string NewPassword, string NewPassword2)
        {
            if (!_userService.CheckPassword(id,OldPassword))
            {
                return Json(new AjaxResult { code = 300, msg = "旧密码不正确！" });
            }
            if (NewPassword != NewPassword2)
            {
                return Json(new AjaxResult { code = 300, msg = "两次密码输入不一致！" });
            }
            _userService.UpdatePassword(id, NewPassword);
            return Json(new AjaxResult { code = 200, msg = "密码修改成功！" });
        }

    }
}
