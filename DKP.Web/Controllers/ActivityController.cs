using DKP.Core.Models;
using DKP.Data;
using DKP.Services.GM;
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
    public class ActivityController : BaseController
    {

        #region 初始化
        private readonly IActivityService _activityService;
        private readonly IServerService _serverService;
        private readonly IDictionaryService _dictionaryService;

        public class ActType
        {
            public static string Cz = "CzType";
            public static string Xh = "XhType";
            public static string Dl = "DlType";
            public static string Ph = "PhType";
            public static string Dh = "DhType";
            public static string Tj = "TjType";
            public static string Zs = "ZsType";
        };

        public ActivityController(IActivityService activityService, IServerService serverService, IDictionaryService dictionaryService)
        {
            _activityService = activityService;
            _serverService = serverService;
            _dictionaryService = dictionaryService;
        } 
        #endregion

        #region 通用方法
        public ActionResult Delete(int id)
        {
            var jsonModel = _activityService.DeleteInfo(id);
            if (jsonModel.status)
            {
                return Success("删除成功！");
            }
            else
            {
                return Error("删除失败：" + jsonModel.msg);
            }
        }

        public ActionResult Send(int id)
        {
            var obj = _activityService.GetInfoById(id);
            if (obj.Status == 1)
            {
                return Error("请勿重复发送！");
            }
            var jsonModel = _activityService.SendActivity(id);
            if (jsonModel.status)
            {
                return Success("发送成功");
            }
            else
            {
                return Error("发送失败：" + jsonModel.msg);
            }
        }

        #endregion

        #region 充值类

        /// <summary>
        /// 充值类活动
        /// </summary>
        /// <returns></returns>
        public ActionResult RechargeIndex()
        {
            return View();
        }
        public ActionResult RechargeList(string search, NameValueCollection forms, PageParam pageParam)
        {
            var activities = _activityService.getAllInfo(Request.QueryString, pageParam, ActType.Cz);
            var results = new List<object>();
            //数据格式化
            foreach (var n in activities)
            {
                results.Add(new
                {
                    IdOp = n.Id,
                    Id = n.Id,
                    Name = n.Name,
                    Type = _dictionaryService.GetDicNameByCode(n.Type),
                    Start = n.StartDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    End = n.EndDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    Desc = n.Desc,
                    Status = n.Status == 1 ? "<span class='label label-primary'>已发送</span>" : "<span class='label label-default'>未发送</span>"
                });
            }
            var json = new
            {
                total = activities.total,
                rows = results
            };
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RechargeDetailIndex(int id)
        {
            ViewBag.Id = id;
            return View();
        }
        public ActionResult RechargeDetailList(string search, NameValueCollection forms, PageParam pageParam, int id)
        {
            var activities = _activityService.getAll(Request.QueryString, pageParam, ActType.Cz, id);
            var results = new List<object>();
            //数据格式化
            foreach (var n in activities)
            {
                results.Add(new
                {
                    IdOp = n.Id,
                    Id = n.Id,
                    Name = n.Name,
                    Type = _dictionaryService.GetDicNameByCode(n.Type),
                    Start = n.StartDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    End = n.EndDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    Desc = n.Desc,
                    Status = n.Status == 1 ? "<span class='label label-primary'>已发送</span>" : "<span class='label label-default'>未发送</span>"
                });
            }
            var json = new
            {
                total = activities.total,
                rows = results
            };
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RechargeAdd(int? pid)
        {
            var model = new Activity();
            if(pid != null)
            {
                var parent = _activityService.GetInfoById(pid.Value);
                model.ActType = parent.ActType;
                model.StartDate = parent.StartDate;
                model.EndDate = parent.EndDate;
                model.Type = parent.Type;
                model.Name = parent.Name;
                model.Desc = parent.Desc;
                model.ActivityInfoId = parent.Id;
            }
            ViewData.Model = model;
            ViewBag.Types = _dictionaryService.GetDictionaryByCode("CzType");
            return View();
        }

        [HttpPost]
        public ActionResult RechargeAdd(Activity obj, List<ActivityServer> smi, List<ActivityItem> mi)
        {
            if (smi == null || smi.Count() == 0)
            {
                return Error("请添加服务器！");
            }
            if (mi == null || mi.Count() == 0)
            {
                return Error("请添加物品！");
            }
            obj.ActivityItems = mi;
            obj.ActivityServers = smi;
            obj.ActType = ActType.Cz;
            _activityService.Add(obj);
            string msg = "添加成功！";
            var jsonModel = _activityService.SendActivity(obj.ActivityInfoId);
            return Success(jsonModel.status ? msg : (msg + "发送失败：" + jsonModel.msg));
        }
        public ActionResult RechargeEdit(int id)
        {
            ViewData.Model = _activityService.GetById(id);
            ViewBag.Types = _dictionaryService.GetDictionaryByCode("CzType");
            return View();
        }

        [HttpPost]
        public ActionResult RechargeEdit(Activity obj, List<ActivityServer> smi, List<ActivityItem> mi)
        {
            if (smi == null || smi.Count() == 0)
            {
                return Error("请添加服务器！");
            }
            if (mi == null || mi.Count() == 0)
            {
                return Error("请添加物品！");
            }
            _activityService.Update(obj, mi, smi);
            string msg = "修改成功！";
            var jsonModel = _activityService.SendActivity(obj.ActivityInfoId);
            return Success(jsonModel.status ? msg : (msg + "发送失败：" + jsonModel.msg));
        }

        public ActionResult RechargeDelete(int id)
        {
            var jsonModel = _activityService.Delete(id);
            if (jsonModel.status)
            {
                return Success("删除成功！");
            }
            else
            {
                return Error("删除失败：" + jsonModel.msg);
            }
        }
        #endregion

        #region 消耗类

        /// <summary>
        /// 消耗类活动
        /// </summary>
        /// <returns></returns>
        public ActionResult ConsumeIndex()
        {
            return View();
        }

        public ActionResult ConsumeList(string search, NameValueCollection forms, PageParam pageParam)
        {
            var activities = _activityService.getAllInfo(Request.QueryString, pageParam, ActType.Xh);
            var results = new List<object>();
            //数据格式化
            foreach (var n in activities)
            {
                results.Add(new
                {
                    IdOp = n.Id,
                    Id = n.Id,
                    Name = n.Name,
                    Type = _dictionaryService.GetDicNameByCode(n.Type),
                    Start = n.StartDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    End = n.EndDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    Desc = n.Desc,
                    Status = n.Status == 1 ? "<span class='label label-primary'>已发送</span>" : "<span class='label label-default'>未发送</span>"
                });
            }
            var json = new
            {
                total = activities.total,
                rows = results
            };
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ConsumeDetailIndex(int id)
        {
            ViewBag.Id = id;
            return View();
        }

        public ActionResult ConsumeDetailList(string search, NameValueCollection forms, PageParam pageParam, int id)
        {
            var activities = _activityService.getAll(Request.QueryString, pageParam, ActType.Xh,id);
            var results = new List<object>();
            //数据格式化
            foreach (var n in activities)
            {
                results.Add(new
                {
                    IdOp = n.Id,
                    Id = n.Id,
                    Name = n.Name,
                    Type = _dictionaryService.GetDicNameByCode(n.Type),
                    Start = n.StartDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    End = n.EndDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    Desc = n.Desc,
                    Status = n.Status == 1 ? "<span class='label label-primary'>已发送</span>" : "<span class='label label-default'>未发送</span>"
                });
            }
            var json = new
            {
                total = activities.total,
                rows = results
            };
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ConsumeAdd(int? pid)
        {
            var model = new Activity();
            if (pid != null)
            {
                var parent = _activityService.GetInfoById(pid.Value);
                model.ActType = parent.ActType;
                model.StartDate = parent.StartDate;
                model.EndDate = parent.EndDate;
                model.Type = parent.Type;
                model.Name = parent.Name;
                model.Desc = parent.Desc;
                model.ActivityInfoId = parent.Id;
            }
            ViewData.Model = model;
            ViewBag.Types = _dictionaryService.GetDictionaryByCode("XhType");
            return View();
        }

        [HttpPost]
        public ActionResult ConsumeAdd(Activity obj, List<ActivityServer> smi, List<ActivityItem> mi)
        {
            if (smi == null || smi.Count() == 0)
            {
                return Error("请添加服务器！");
            }
            if (mi == null || mi.Count() == 0)
            {
                return Error("请添加物品！");
            }
            obj.ActivityItems = mi;
            obj.ActivityServers = smi;
            obj.ActType = ActType.Xh;
            _activityService.Add(obj);
            string msg = "添加成功！";
            var jsonModel = _activityService.SendActivity(obj.ActivityInfoId);
            return Success(jsonModel.status ? msg : (msg + "发送失败：" + jsonModel.msg));
        }
        public ActionResult ConsumeEdit(int id)
        {
            var a = new Activity();
            ViewData.Model = _activityService.GetById(id);
            ViewBag.Types = _dictionaryService.GetDictionaryByCode("XhType");
            return View();
        }

        [HttpPost]
        public ActionResult ConsumeEdit(Activity obj, List<ActivityServer> smi, List<ActivityItem> mi)
        {
            if (smi == null || smi.Count() == 0)
            {
                return Error("请添加服务器！");
            }
            if (mi == null || mi.Count() == 0)
            {
                return Error("请添加物品！");
            }
            _activityService.Update(obj, mi, smi);
            string msg = "修改成功！";
            var jsonModel = _activityService.SendActivity(obj.ActivityInfoId);
            return Success(jsonModel.status ? msg : (msg + "发送失败：" + jsonModel.msg));
        }

        public ActionResult ConsumeDelete(int id)
        {
            var jsonModel = _activityService.Delete(id);
            if (jsonModel.status)
            {
                return Success("删除成功！");
            }
            else
            {
                return Error("删除失败：" + jsonModel.msg);
            }
        }
        #endregion

        #region 登录类

        /// <summary>
        /// 登录类活动
        /// </summary>
        /// <returns></returns>
        public ActionResult LoginIndex()
        {
            return View();
        }

        public ActionResult LoginList(string search, NameValueCollection forms, PageParam pageParam)
        {
            var activities = _activityService.getAllInfo(Request.QueryString, pageParam, ActType.Dl);
            var results = new List<object>();
            //数据格式化
            foreach (var n in activities)
            {
                results.Add(new
                {
                    IdOp = n.Id,
                    Id = n.Id,
                    Name = n.Name,
                    Type = _dictionaryService.GetDicNameByCode(n.Type),
                    Start = n.StartDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    End = n.EndDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    Desc = n.Desc,
                    Status = n.Status == 1 ? "<span class='label label-primary'>已发送</span>" : "<span class='label label-default'>未发送</span>"
                });
            }
            var json = new
            {
                total = activities.total,
                rows = results
            };
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoginDetailIndex(int id)
        {
            ViewBag.Id = id;
            return View();
        }

        public ActionResult LoginDetailList(string search, NameValueCollection forms, PageParam pageParam, int id)
        {
            var activities = _activityService.getAll(Request.QueryString, pageParam, ActType.Dl, id);
            var results = new List<object>();
            //数据格式化
            foreach (var n in activities)
            {
                results.Add(new
                {
                    IdOp = n.Id,
                    Id = n.Id,
                    Name = n.Name,
                    Type = _dictionaryService.GetDicNameByCode(n.Type),
                    Start = n.StartDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    End = n.EndDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    Desc = n.Desc,
                    Status = n.Status == 1 ? "<span class='label label-primary'>已发送</span>" : "<span class='label label-default'>未发送</span>"
                });
            }
            var json = new
            {
                total = activities.total,
                rows = results
            };
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoginAdd(int? pid)
        {
            var model = new Activity();
            if (pid != null)
            {
                var parent = _activityService.GetInfoById(pid.Value);
                model.ActType = parent.ActType;
                model.StartDate = parent.StartDate;
                model.EndDate = parent.EndDate;
                model.Type = parent.Type;
                model.Name = parent.Name;
                model.Desc = parent.Desc;
                model.ActivityInfoId = parent.Id;
            }
            ViewData.Model = model;
            ViewBag.Types = _dictionaryService.GetDictionaryByCode("DlType");
            return View();
        }

        [HttpPost]
        public ActionResult LoginAdd(Activity obj, List<ActivityServer> smi, List<ActivityItem> mi)
        {
            if (smi == null || smi.Count() == 0)
            {
                return Error("请添加服务器！");
            }
            if (mi == null || mi.Count() == 0)
            {
                return Error("请添加物品！");
            }
            obj.ActivityItems = mi;
            obj.ActivityServers = smi;
            obj.ActType = ActType.Dl;
            _activityService.Add(obj);
            string msg = "添加成功！";
            var jsonModel = _activityService.SendActivity(obj.ActivityInfoId);
            return Success(jsonModel.status ? msg : (msg + "发送失败：" + jsonModel.msg));
        }
        public ActionResult LoginEdit(int id)
        {
            ViewData.Model = _activityService.GetById(id);
            ViewBag.Types = _dictionaryService.GetDictionaryByCode("DlType");
            return View();
        }

        [HttpPost]
        public ActionResult LoginEdit(Activity obj, List<ActivityServer> smi, List<ActivityItem> mi)
        {
            if (smi == null || smi.Count() == 0)
            {
                return Error("请添加服务器！");
            }
            if (mi == null || mi.Count() == 0)
            {
                return Error("请添加物品！");
            }
            _activityService.Update(obj, mi, smi);
            string msg = "修改成功！";
            var jsonModel = _activityService.SendActivity(obj.ActivityInfoId);
            return Success(jsonModel.status ? msg : (msg + "发送失败：" + jsonModel.msg));
        }

        public ActionResult LoginDelete(int id)
        {
            var jsonModel = _activityService.Delete(id);
            if (jsonModel.status)
            {
                return Success("删除成功！");
            }
            else
            {
                return Error("删除失败：" + jsonModel.msg);
            }
        }
        #endregion

        #region 排行类

        /// <summary>
        /// 排行类活动
        /// </summary>
        /// <returns></returns>
        public ActionResult TopIndex()
        {
            return View();
        }

        public ActionResult TopList(string search, NameValueCollection forms, PageParam pageParam)
        {
            var activities = _activityService.getAllInfo(Request.QueryString, pageParam, ActType.Ph);
            var results = new List<object>();
            //数据格式化
            foreach (var n in activities)
            {
                results.Add(new
                {
                    IdOp = n.Id,
                    Id = n.Id,
                    Name = n.Name,
                    Type = _dictionaryService.GetDicNameByCode(n.Type),
                    Start = n.StartDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    End = n.EndDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    Desc = n.Desc,
                    Status = n.Status == 1 ? "<span class='label label-primary'>已发送</span>" : "<span class='label label-default'>未发送</span>"
                });
            }
            var json = new
            {
                total = activities.total,
                rows = results
            };
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TopDetailIndex(int id)
        {
            ViewBag.Id = id;
            return View();
        }

        public ActionResult TopDetailList(string search, NameValueCollection forms, PageParam pageParam, int id)
        {
            var activities = _activityService.getAll(Request.QueryString, pageParam, ActType.Ph, id);
            var results = new List<object>();
            //数据格式化
            foreach (var n in activities)
            {
                results.Add(new
                {
                    IdOp = n.Id,
                    Id = n.Id,
                    Name = n.Name,
                    Type = _dictionaryService.GetDicNameByCode(n.Type),
                    Start = n.StartDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    End = n.EndDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    Desc = n.Desc,
                    Status = n.Status == 1 ? "<span class='label label-primary'>已发送</span>" : "<span class='label label-default'>未发送</span>"
                });
            }
            var json = new
            {
                total = activities.total,
                rows = results
            };
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TopAdd(int? pid)
        {
            var model = new Activity();
            if (pid != null)
            {
                var parent = _activityService.GetInfoById(pid.Value);
                model.ActType = parent.ActType;
                model.StartDate = parent.StartDate;
                model.EndDate = parent.EndDate;
                model.Type = parent.Type;
                model.Name = parent.Name;
                model.Desc = parent.Desc;
                model.ActivityInfoId = parent.Id;
            }
            ViewData.Model = model;
            ViewBag.Types = _dictionaryService.GetDictionaryByCode("PhType");
            return View();
        }

        [HttpPost]
        public ActionResult TopAdd(Activity obj, List<ActivityServer> smi, List<ActivityItem> mi)
        {
            if (smi == null || smi.Count() == 0)
            {
                return Error("请添加服务器！");
            }
            if (mi == null || mi.Count() == 0)
            {
                return Error("请添加物品！");
            }
            obj.ActivityItems = mi;
            obj.ActivityServers = smi;
            obj.ActType = ActType.Ph;
            _activityService.Add(obj);
            string msg = "添加成功！";
            var jsonModel = _activityService.SendActivity(obj.ActivityInfoId);
            return Success(jsonModel.status ? msg : (msg + "发送失败：" + jsonModel.msg));
        }

        public ActionResult TopEdit(int id)
        {
            ViewData.Model = _activityService.GetById(id);
            ViewBag.Types = _dictionaryService.GetDictionaryByCode("PhType");
            return View();
        }

        [HttpPost]
        public ActionResult TopEdit(Activity obj, List<ActivityServer> smi, List<ActivityItem> mi)
        {
            if (smi == null || smi.Count() == 0)
            {
                return Error("请添加服务器！");
            }
            if (mi == null || mi.Count() == 0)
            {
                return Error("请添加物品！");
            }
            _activityService.Update(obj, mi, smi);
            string msg = "修改成功！";
            var jsonModel = _activityService.SendActivity(obj.ActivityInfoId);
            return Success(jsonModel.status ? msg : (msg + "发送失败：" + jsonModel.msg));
        }

        public ActionResult TopDelete(int id)
        {
            var jsonModel = _activityService.Delete(id);
            if (jsonModel.status)
            {
                return Success("删除成功！");
            }
            else
            {
                return Error("删除失败：" + jsonModel.msg);
            }
        }
        #endregion

        #region 兑换类

        /// <summary>
        /// 兑换类活动
        /// </summary>
        /// <returns></returns>
        public ActionResult ExchangeIndex()
        {
            return View();
        }

        public ActionResult ExchangeList(string search, NameValueCollection forms, PageParam pageParam)
        {
            var activities = _activityService.getAllInfo(Request.QueryString, pageParam, ActType.Dh);
            var results = new List<object>();
            //数据格式化
            foreach (var n in activities)
            {
                results.Add(new
                {
                    IdOp = n.Id,
                    Id = n.Id,
                    Name = n.Name,
                    Type = _dictionaryService.GetDicNameByCode(n.Type),
                    Start = n.StartDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    End = n.EndDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    Desc = n.Desc,
                    Status = n.Status == 1 ? "<span class='label label-primary'>已发送</span>" : "<span class='label label-default'>未发送</span>"
                });
            }
            var json = new
            {
                total = activities.total,
                rows = results
            };
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExchangeDetailIndex(int id)
        {
            ViewBag.Id = id;
            return View();
        }

        public ActionResult ExchangeDetailList(string search, NameValueCollection forms, PageParam pageParam,int id)
        {
            var activities = _activityService.getAll(Request.QueryString, pageParam, ActType.Dh,id);
            var results = new List<object>();
            //数据格式化
            foreach (var n in activities)
            {
                results.Add(new
                {
                    IdOp = n.Id,
                    Id = n.Id,
                    Name = n.Name,
                    Type = _dictionaryService.GetDicNameByCode(n.Type),
                    Start = n.StartDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    End = n.EndDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    Desc = n.Desc,
                    Status = n.Status == 1 ? "<span class='label label-primary'>已发送</span>" : "<span class='label label-default'>未发送</span>"
                });
            }
            var json = new
            {
                total = activities.total,
                rows = results
            };
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExchangeAdd(int? pid)
        {
            var model = new Activity();
            if (pid != null)
            {
                var parent = _activityService.GetInfoById(pid.Value);
                model.ActType = parent.ActType;
                model.StartDate = parent.StartDate;
                model.EndDate = parent.EndDate;
                model.Type = parent.Type;
                model.Name = parent.Name;
                model.Desc = parent.Desc;
                model.ActivityInfoId = parent.Id;
            }
            ViewData.Model = model;
            ViewBag.Types = _dictionaryService.GetDictionaryByCode("DhType");
            return View();
        }

        [HttpPost]
        public ActionResult ExchangeAdd(Activity obj, List<ActivityServer> smi, List<ActivityItem> mi)
        {
            if (smi == null || smi.Count() == 0)
            {
                return Error("请添加服务器！");
            }
            if (mi == null || mi.Count() == 0)
            {
                return Error("请添加物品！");
            }
            if (obj.Type == "SingleExchange" && mi.Count() > 1)
            {
                return Error("单项兑换请勿添加多个兑换物品！");
            }
            if (obj.Type == "MultiExchange" && mi.Count() == 1)
            {
                return Error("多项兑换请添加多个兑换物品！");
            }
            obj.ActivityItems = mi;
            obj.ActivityServers = smi;
            obj.ActType = ActType.Dh;
            _activityService.Add(obj);
            string msg = "添加成功！";
            var jsonModel = _activityService.SendActivity(obj.ActivityInfoId);
            return Success(jsonModel.status ? msg : (msg + "发送失败：" + jsonModel.msg));
        }

        public ActionResult ExchangeEdit(int id)
        {
            ViewData.Model = _activityService.GetById(id);
            ViewBag.Types = _dictionaryService.GetDictionaryByCode("DhType");
            return View();
        }

        [HttpPost]
        public ActionResult ExchangeEdit(Activity obj, List<ActivityServer> smi, List<ActivityItem> mi)
        {
            if (smi == null || smi.Count() == 0)
            {
                return Error("请添加服务器！");
            }
            if (mi == null || mi.Count() == 0)
            {
                return Error("请添加物品！");
            }
            if (obj.Type == "SingleExchange" && mi.Count() > 1)
            {
                return Error("单项兑换请勿添加多个兑换物品！");
            }
            if (obj.Type == "MultiExchange" && mi.Count() == 1)
            {
                return Error("多项兑换请添加多个兑换物品！");
            }
            _activityService.Update(obj, mi, smi);
            string msg = "修改成功！";
            var jsonModel = _activityService.SendActivity(obj.ActivityInfoId);
            return Success(jsonModel.status ? msg : (msg + "发送失败：" + jsonModel.msg));
        }

        public ActionResult ExchangeDelete(int id)
        {
            var jsonModel = _activityService.Delete(id);
            if (jsonModel.status)
            {
                return Success("删除成功！");
            }
            else
            {
                return Error("删除失败：" + jsonModel.msg);
            }
        }
        #endregion

        #region 条件类

        /// <summary>
        /// 条件类活动
        /// </summary>
        /// <returns></returns>
        public ActionResult ConditionIndex()
        {
            return View();
        }

        public ActionResult ConditionList(string search, NameValueCollection forms, PageParam pageParam)
        {
            var activities = _activityService.getAllInfo(Request.QueryString, pageParam, ActType.Tj);
            var results = new List<object>();
            //数据格式化
            foreach (var n in activities)
            {
                results.Add(new
                {
                    IdOp = n.Id,
                    Id = n.Id,
                    Name = n.Name,
                    Type = _dictionaryService.GetDicNameByCode(n.Type),
                    Start = n.StartDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    End = n.EndDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    Desc = n.Desc,
                    Status = n.Status == 1 ? "<span class='label label-primary'>已发送</span>" : "<span class='label label-default'>未发送</span>"
                });
            }
            var json = new
            {
                total = activities.total,
                rows = results
            };
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ConditionDetailIndex(int id)
        {
            ViewBag.Id = id;
            return View();
        }

        public ActionResult ConditionDetailList(string search, NameValueCollection forms, PageParam pageParam, int id)
        {
            var activities = _activityService.getAll(Request.QueryString, pageParam, ActType.Tj, id);
            var results = new List<object>();
            //数据格式化
            foreach (var n in activities)
            {
                results.Add(new
                {
                    IdOp = n.Id,
                    Id = n.Id,
                    Name = n.Name,
                    Type = _dictionaryService.GetDicNameByCode(n.Type),
                    Start = n.StartDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    End = n.EndDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    Desc = n.Desc,
                    Status = n.Status == 1 ? "<span class='label label-primary'>已发送</span>" : "<span class='label label-default'>未发送</span>"
                });
            }
            var json = new
            {
                total = activities.total,
                rows = results
            };
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ConditionAdd(int? pid)
        {
            var model = new Activity();
            if (pid != null)
            {
                var parent = _activityService.GetInfoById(pid.Value);
                model.ActType = parent.ActType;
                model.StartDate = parent.StartDate;
                model.EndDate = parent.EndDate;
                model.Type = parent.Type;
                model.Name = parent.Name;
                model.Desc = parent.Desc;
                model.ActivityInfoId = parent.Id;
            }
            ViewData.Model = model;
            ViewBag.Types = _dictionaryService.GetDictionaryByCode("TjType");
            return View();
        }

        [HttpPost]
        public ActionResult ConditionAdd(Activity obj, List<ActivityServer> smi, List<ActivityItem> mi)
        {
            if (smi == null || smi.Count() == 0)
            {
                return Error("请添加服务器！");
            }
            if (mi == null || mi.Count() == 0)
            {
                return Error("请添加物品！");
            }
            obj.ActivityItems = mi;
            obj.ActivityServers = smi;
            obj.ActType = ActType.Tj;
            _activityService.Add(obj);
            string msg = "添加成功！";
            var jsonModel = _activityService.SendActivity(obj.ActivityInfoId);
            return Success(jsonModel.status ? msg : (msg + "发送失败：" + jsonModel.msg));
        }
        public ActionResult ConditionEdit(int id)
        {
            ViewData.Model = _activityService.GetById(id);
            ViewBag.Types = _dictionaryService.GetDictionaryByCode("TjType");
            return View();
        }

        [HttpPost]
        public ActionResult ConditionEdit(Activity obj, List<ActivityServer> smi, List<ActivityItem> mi)
        {
            if (smi == null || smi.Count() == 0)
            {
                return Error("请添加服务器！");
            }
            if (mi == null || mi.Count() == 0)
            {
                return Error("请添加物品！");
            }
            _activityService.Update(obj, mi, smi);
            string msg = "修改成功！";
            var jsonModel = _activityService.SendActivity(obj.ActivityInfoId);
            return Success(jsonModel.status ? msg : (msg + "发送失败：" + jsonModel.msg));
        }

        public ActionResult ConditionDelete(int id)
        {
            var jsonModel = _activityService.Delete(id);
            if (jsonModel.status)
            {
                return Success("删除成功！");
            }
            else
            {
                return Error("删除失败：" + jsonModel.msg);
            }
        }
        #endregion

        #region 展示类

        /// <summary>
        /// 展示类活动
        /// </summary>
        /// <returns></returns>
        public ActionResult ShowIndex()
        {
            return View();
        }
        public ActionResult ShowList(string search, NameValueCollection forms, PageParam pageParam)
        {
            var activities = _activityService.getAllInfo(Request.QueryString, pageParam, ActType.Zs);
            var results = new List<object>();
            //数据格式化
            foreach (var n in activities)
            {
                results.Add(new
                {
                    IdOp = n.Id,
                    Id = n.Id,
                    Name = n.Name,
                    Type = _dictionaryService.GetDicNameByCode(n.Type),
                    Start = n.StartDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    End = n.EndDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    Desc = n.Desc,
                    Status = n.Status == 1 ? "<span class='label label-primary'>已发送</span>" : "<span class='label label-default'>未发送</span>"
                });
            }
            var json = new
            {
                total = activities.total,
                rows = results
            };
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ShowDetailIndex(int id)
        {
            ViewBag.Id = id;
            return View();
        }
        public ActionResult ShowDetailList(string search, NameValueCollection forms, PageParam pageParam, int id)
        {
            var activities = _activityService.getAll(Request.QueryString, pageParam, ActType.Zs, id);
            var results = new List<object>();
            //数据格式化
            foreach (var n in activities)
            {
                results.Add(new
                {
                    IdOp = n.Id,
                    Id = n.Id,
                    Name = n.Name,
                    Type = _dictionaryService.GetDicNameByCode(n.Type),
                    Start = n.StartDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    End = n.EndDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    Desc = n.Desc,
                    Status = n.Status == 1 ? "<span class='label label-primary'>已发送</span>" : "<span class='label label-default'>未发送</span>"
                });
            }
            var json = new
            {
                total = activities.total,
                rows = results
            };
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ShowAdd(int? pid)
        {
            var model = new Activity();
            if (pid != null)
            {
                var parent = _activityService.GetInfoById(pid.Value);
                model.ActType = parent.ActType;
                model.StartDate = parent.StartDate;
                model.EndDate = parent.EndDate;
                model.Type = parent.Type;
                model.Name = parent.Name;
                model.Desc = parent.Desc;
                model.ActivityInfoId = parent.Id;
            }
            ViewData.Model = model;
            ViewBag.Types = _dictionaryService.GetDictionaryByCode("ZsType");
            return View();
        }

        [HttpPost]
        public ActionResult ShowAdd(Activity obj, List<ActivityServer> smi, List<ActivityItem> mi)
        {
            if (smi == null || smi.Count() == 0)
            {
                return Error("请添加服务器！");
            }
            if (mi == null || mi.Count() == 0)
            {
                return Error("请添加物品！");
            }
            obj.ActivityItems = mi;
            obj.ActivityServers = smi;
            obj.ActType = ActType.Zs;
            _activityService.Add(obj);
            string msg = "添加成功！";
            var jsonModel = _activityService.SendActivity(obj.ActivityInfoId);
            return Success(jsonModel.status ? msg : (msg + "发送失败：" + jsonModel.msg));
        }
        public ActionResult ShowEdit(int id)
        {
            ViewData.Model = _activityService.GetById(id);
            ViewBag.Types = _dictionaryService.GetDictionaryByCode("ZsType");
            return View();
        }

        [HttpPost]
        public ActionResult ShowEdit(Activity obj, List<ActivityServer> smi, List<ActivityItem> mi)
        {
            if (smi == null || smi.Count() == 0)
            {
                return Error("请添加服务器！");
            }
            if (mi == null || mi.Count() == 0)
            {
                return Error("请添加物品！");
            }
            _activityService.Update(obj, mi, smi);
            string msg = "修改成功！";
            var jsonModel = _activityService.SendActivity(obj.ActivityInfoId);
            return Success(jsonModel.status ? msg : (msg + "发送失败：" + jsonModel.msg));
        }

        public ActionResult ShowDelete(int id)
        {
            var jsonModel = _activityService.Delete(id);
            if (jsonModel.status)
            {
                return Success("删除成功！");
            }
            else
            {
                return Error("删除失败：" + jsonModel.msg);
            }
        }
        #endregion
    }
}
