using DKP.Core;
using DKP.Core.Models;
using DKP.Data;
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
    public class DictionaryController : BaseController
    {
        private readonly IDictionaryService _dictionaryService;
        private readonly IWorkContext _workContext;
        public DictionaryController(
           IDictionaryService dictionaryService,
           IWorkContext workContext)
        {
            _dictionaryService = dictionaryService;
            _workContext = workContext;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List(string search, NameValueCollection forms,PageParam pageParam)
        {
            var dics = _dictionaryService.GetAllDictionarys(forms, pageParam);
            var rst = new List<object>();
            foreach(var d in dics)
            {
                var r = new
                {
                    Id = d.Id,
                    Name = d.Name,
                    Code = d.Code,
                    Group = d.DictionaryGroup.Name
                };
                rst.Add(r);
            }
            var json = new
            {
                total = dics.total,
                rows = rst
            };
            return Json(json,JsonRequestBehavior.AllowGet);
        }

        public ActionResult Add()
        {
            ViewBag.Group = _dictionaryService.GetGroups();
            ViewData.Model = new Dictionary();
            return View();
        }

        [HttpPost]
        public ActionResult Add(Dictionary dic)
        {
            _dictionaryService.Create(dic);
            return Success("添加成功");
        }

        public ActionResult Edit(int id)
        {
            var dic = _dictionaryService.GetDicById(id);
            ViewBag.Group = _dictionaryService.GetGroups();
            ViewData.Model = dic;
            return View();
        }

        [HttpPost]
        public ActionResult Edit(Dictionary dic)
        {
            _dictionaryService.Update(dic);
            return Success("修改成功");
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            _dictionaryService.DeleteDictionary(id);
            return Success("删除成功");
        }
    }
}
