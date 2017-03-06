using System.Drawing;
using System.Text;
using System.Web.Mvc;
using DKP.Web.Framework.Filters;
using DKP.Web.Framework.Results;
using DKP.Web.Framework.Security.Captcha;
using DKP.Core.Models;
using DKP.Data.Models;

namespace DKP.Web.Framework.Controllers
{
    [ExceptionFilter]
    [AuthFilter]
    [CompressFilter]
    [FormFilter]
    [PermissionFilter]
    public abstract class BaseController : Controller
    {
        #region 返回结果 ok / error / timeout

        //public enum ReloadMode
        //{
        //    NavTab,
        //    Dialog,
        //    Div
        //}

        //public enum StatusCode
        //{
        //    Success = 200,
        //    Error = 300,
        //    Timeout = 301
        //}

        //public ActionResult DialogErrorMsg(string message, string closeType)
        //{
        //    ViewBag.message = message;
        //    ViewBag.closeType = closeType;
        //    return View("/Views/Shared/DialogErrorView.cshtml");
        //}

        ///// <summary>
        ///// 返回一个成功信息Json，以便传过客户端使用。
        ///// </summary>
        ///// <param name="statusCode">状态</param>
        ///// <param name="message">显示成功信息内容</param>
        ///// <param name="tabId">操作成功后要刷新的Tab id(多个可以用“,”隔开)</param>
        ///// <param name="dialogId">操作成功后要刷新的Dialog id(多个可以用“,”隔开)</param>
        ///// <param name="divId">操作成功后要刷新的元素 id(多个可以用“,”隔开)</param>
        ///// <param name="closeCurrent">是否关闭当前页面</param>
        ///// <returns></returns>
        //[NonAction]
        //protected JsonResult ShowMsg(int statusCode, string message, string tabId, string dialogId, string divId, bool closeCurrent = true)
        //{
        //    var jsonData = new
        //    {
        //        statusCode = statusCode,
        //        message = message,
        //        tabid = tabId,
        //        dialogid = dialogId,
        //        divid = divId,
        //        closeCurrent = closeCurrent
        //    };

        //    return Json(jsonData, "application/Json");
        //}

        ///// <summary>
        ///// 提示信息
        ///// </summary>
        ///// <param name="id">元素id</param>
        ///// <param name="message">提示信息</param>
        ///// <param name="closeCurrent">是否关闭当前页面</param>
        ///// <param name="reloadMode">操作完成之后，需要刷新的元素模式</param>
        ///// <param name="statusCode">状态编码</param>
        ///// <returns></returns>
        //[NonAction]
        //protected JsonResult ShowMsg(string id, string message, int reloadMode, bool closeCurrent, int statusCode)
        //{
        //    switch (reloadMode)
        //    {
        //        case (int)ReloadMode.Div:
        //            return ShowMsg(statusCode, message, "", "", id, closeCurrent);
        //        case (int)ReloadMode.Dialog:
        //            return ShowMsg(statusCode, message, "", id, "", closeCurrent);
        //        case (int)ReloadMode.NavTab:
        //        default:
        //            return ShowMsg(statusCode, message, id, "", "", closeCurrent);
        //    }
        //}

        ///// <summary>
        ///// 提示信息
        ///// </summary>
        ///// <param name="id">元素id</param>
        ///// <returns></returns>
        //[NonAction]
        //protected JsonResult SuccessMsg(string id) { return ShowMsg(id, "操作成功", (int)ReloadMode.NavTab, true, (int)StatusCode.Success); }


        ///// <summary>
        ///// 提示信息
        ///// </summary>
        ///// <param name="id">元素id</param>
        ///// <param name="closeCurrent">是否关闭当前页面</param>
        ///// <returns></returns>
        //[NonAction]
        //protected JsonResult SuccessMsg(string id, bool closeCurrent) { return ShowMsg(id, "操作成功", (int)ReloadMode.NavTab, closeCurrent, (int)StatusCode.Success); }

        ///// <summary>
        ///// 提示信息
        ///// </summary>
        ///// <param name="id">元素id</param>
        ///// <param name="message">提示信息</param>
        ///// <param name="closeCurrent">是否关闭当前页面</param>
        ///// <returns></returns>
        //[NonAction]
        //protected JsonResult SuccessMsg(string id, string message, bool closeCurrent) { return ShowMsg(id, message, (int)ReloadMode.NavTab, closeCurrent, (int)StatusCode.Success); }

        ///// <summary>
        ///// 提示信息
        ///// </summary>
        ///// <param name="id">元素id</param>
        ///// <param name="closeCurrent">是否关闭当前页面</param>
        ///// <returns></returns>
        //[NonAction]
        //protected JsonResult SuccessDivMsg(string id, bool closeCurrent) { return ShowMsg(id, "操作成功", (int)ReloadMode.Div, closeCurrent, (int)StatusCode.Success); }

        ///// <summary>
        ///// 提示信息
        ///// </summary>
        ///// <param name="id">元素id</param>
        ///// <param name="closeCurrent">是否关闭当前页面</param>
        ///// <returns></returns>
        //[NonAction]
        //protected JsonResult ErrorMsg(string message, bool closeCurrent) { return ShowMsg("", message, (int)ReloadMode.NavTab, closeCurrent, (int)StatusCode.Error); }


        ///// <summary>
        ///// 错误提示信息
        ///// </summary>
        ///// <param name="message">提示信息</param>
        ///// <returns></returns>
        //[NonAction]
        //protected JsonResult ErrorMsg(string message) { return ShowMsg("", message, (int)ReloadMode.NavTab, false, (int)StatusCode.Error); }


        //[NonAction]
        //protected ContentResult UploadSuccess(string navTabId)
        //{
        //    string strJson = "{statusCode:'200',message:'上传成功',navTabId:'" + navTabId + "',callbackType:'closeCurrent'}";

        //    return Content(
        //        "<script language='javascript'>" +
        //            "if(window.parent.donecallback) " +
        //                "window.parent.donecallback(" + strJson + ");" +
        //        "</script>"
        //    );
        //}

        [NonAction]
        protected ActionResult ErrorView(string msg,int code)
        {
            return View("ErrorView", new AjaxResult(msg,code));
        }

        [NonAction]
        protected ActionResult Success(string msg)
        {
            return Json(new AjaxResult { code = 200, msg = msg });
        }

        [NonAction]
        protected ActionResult Error(string msg)
        {
            return Json(new AjaxResult { code = 300, msg = msg });
        }

        [NonAction]
        protected ActionResult ListResult(JsonModel json)
        {
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        #endregion

        protected override void HandleUnknownAction(string actionName)
        {
            Response.Write("<script>window.parent.showMsg('error','对不起，【"+ actionName + "】找不到此操作');$('#modal').modal('hide');</script>");
        }
    }
}
