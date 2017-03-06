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
    public class PermissionController : BaseController
    {
        private readonly IPermissionService _permissionService;
        private readonly IWorkContext _workContext;

        public PermissionController(
            IPermissionService permissionService,
           IWorkContext workContext)
        {
            _permissionService = permissionService;
            _workContext = workContext;
        }

        public ActionResult Index()
        {
            ViewData.Model = _permissionService.GetAllPermissions().Where(p=>p.Level == 0).ToList();
            return View();
        }

        public ActionResult Add(int? id)
        {
            if (id != null)
            {
                ViewBag.Parent = _permissionService.GetPermissionById(id.Value);
            }
            ViewData.Model = new DKP.Data.Permission();
            return View();
        }
        [HttpPost]
        public ActionResult Add(DKP.Data.Permission permission)
        {
            _permissionService.AddPermission(permission);
            return Json(new AjaxResult("权限添加成功！"));
        }

        public ActionResult Edit(int? id)
        {
            if(id == null)
            {
                return ErrorView("请选择要修改的权限！", 300);
            }
            ViewData.Model = _permissionService.GetPermissionById(id.Value);
            return View();
        }
        [HttpPost]
        public ActionResult Edit(DKP.Data.Permission permission)
        {
            _permissionService.updatePermission(permission);
            return Json(new AjaxResult("权限修改成功！"));
        }

        [HttpPost]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return Json(new AjaxResult("请选择要删除的权限！", 300));
            }
            _permissionService.deletePermission(id.Value);
            return Json(new AjaxResult("权限删除成功！"));
        }
    }
}
