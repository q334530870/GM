using DKP.Core;
using DKP.Core.Models;
using DKP.Data;
using DKP.Services.Security;
using DKP.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DKP.Web.Controllers
{
    public class RoleController : BaseController
    {
        private readonly IRoleService _roleService;
        private readonly IPermissionService _permissionService;
        private readonly IWorkContext _workContext;

        public RoleController(
            IRoleService roleService,
            IPermissionService permissionService,
           IWorkContext workContext)
        {
            _roleService = roleService;
            _permissionService = permissionService;
            _workContext = workContext;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List(NameValueCollection forms, PageParam pageParam)
        {
            var roles = _roleService.GetAllRoles(forms, pageParam);
            //解决循环引用
            foreach(var r in roles)
            {
                r.Users.Clear();
                r.Permissions.Clear();
            }
            var json = new
            {
                total = roles.total,
                rows = roles
            };
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Add()
        {
            ViewData.Model = new Role();
            ViewBag.Permissions = _permissionService.GetAllPermissions().Where(p => p.Level == 0).ToList();
            return View();
        }
        [HttpPost]
        public ActionResult Add(Role role,string ids)
        {
            var permissions = ids.Split(new char []{ ',' }, StringSplitOptions.RemoveEmptyEntries);
            _roleService.InsterRole(role, permissions);
            return Json(new AjaxResult { code = 200, msg = "角色添加成功！" });
        }

        public ActionResult Edit(int id)
        {
            var role = _roleService.GetRoleById(id);
            ViewBag.Permissions = _permissionService.GetAllPermissions().Where(p => p.Level == 0).ToList();
            ViewData.Model = role;
            return View();
        }
        [HttpPost]
        public ActionResult Edit(Role role, string ids)
        {
            role.Permissions = _permissionService.GetAllPermissions().Where(g => g.Roles.Where(r => r.Id == role.Id).Count() > 0).ToList();
            var permissions = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            _roleService.UpdateRole(role, permissions);
            return Json(new AjaxResult { code = 200, msg = "角色修改成功！" });
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            _roleService.DeleteRole(id);
            return Json(new AjaxResult { code = 200, msg = "角色删除成功！" });
        }

    }
}
