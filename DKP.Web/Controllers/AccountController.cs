using DKP.Core;
using DKP.Data;
using DKP.Services.Security;
using DKP.Services.Security.Models;
using DKP.Web.Framework.Filters;
using System;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace DKP.Web.Controllers
{
    [ExceptionFilter]
    [CompressFilter]
    public class AccountController:Controller
    {
        private readonly IUserService _userService;
        private readonly IWorkContext _workContext;

        public AccountController(
            IUserService userService,
            IWorkContext workContext)
        {
            _userService = userService;
            _workContext = workContext;
        }
        // GET: /Account/

        public ActionResult Login()
        {
            ViewBag.ModelState = true;
            return View(new User());
        }

        [HttpPost]
        public ActionResult Login(User user)
        {
            var loginResult = _userService.Login(user.UserName, user.Password);
            if (loginResult.LoginResultCode == LoginResultCode.Successful)
            {
                _workContext.CurrentUserId = loginResult.User.Id;
                return RedirectToAction("Index","Home");
            }
            ViewBag.ErrorMessage = loginResult.Message;
            return View(user);
        }

        [AuthFilter]
        public ActionResult Logout()
        {
            System.Web.HttpContext.Current.Application.Clear();
            return RedirectToAction("Login");
        }

        [AuthFilter]
        public ActionResult Avatar()
        {
            return View();
        }

        [AuthFilter]
        public ActionResult UploadAvatar(HttpPostedFileBase __source,HttpPostedFileBase __avatar1, HttpPostedFileBase __avatar2, HttpPostedFileBase __avatar3)
        {
            string dicPath = "~/Uploads/Avatar";
            var user = _userService.GetById(_workContext.CurrentUser.UserId);
            string guid = user.Id + "-" + Guid.NewGuid().ToString();


            if (!Directory.Exists(Server.MapPath(dicPath)))
            {
                Directory.CreateDirectory(Server.MapPath(dicPath));
            }
            if (__source != null)
            {
                if (!Directory.Exists(Server.MapPath(dicPath+"/old")))
                {
                    Directory.CreateDirectory(Server.MapPath(dicPath + "/old"));
                }
                string oldFileName = guid + ".png";
                __source.SaveAs(Server.MapPath(dicPath + "/old/") + oldFileName);
            }
            if(__avatar1 != null)
            {
                if (!Directory.Exists(Server.MapPath(dicPath + "/new")))
                {
                    Directory.CreateDirectory(Server.MapPath(dicPath + "/new"));
                }
                string newFileName = guid + "-32.png";
                __avatar1.SaveAs(Server.MapPath(dicPath + "/new/") + newFileName);
            }
            if (__avatar2 != null)
            {
                if (!Directory.Exists(Server.MapPath(dicPath + "/new")))
                {
                    Directory.CreateDirectory(Server.MapPath(dicPath + "/new"));
                }
                string newFileName = guid + "-64.png";
                __avatar2.SaveAs(Server.MapPath(dicPath + "/new/") + newFileName);
            }
            if (__avatar3 != null)
            {
                if (!Directory.Exists(Server.MapPath(dicPath + "/new")))
                {
                    Directory.CreateDirectory(Server.MapPath(dicPath + "/new"));
                }
                string newFileName = guid + "-128.png";
                __avatar3.SaveAs(Server.MapPath(dicPath + "/new/") + newFileName);
            }
            user.Avatar = dicPath + "/new/" + guid;
            _userService.Update(user);
            _workContext.CurrentUser.Avatar = user.Avatar;
            return Json(new
            {
                success = true//该名/值对是必须定义的，表示上传成功
                //sourceUrl = "/Content/img/avatar.jpg",
                //avatarUrls = new string[]{ "/Content/img/avatar.jpg" }
            });
        }

        [AuthFilter]
        public ActionResult Contact()
        {
            return View();
        }

    }
}
