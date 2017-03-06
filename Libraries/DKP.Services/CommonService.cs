using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using DKP.Core.Infrastructure;
using DKP.Core;
using DKP.Core.Data;
using DKP.Services.Security;
using DKP.Data;
using DKP.Data.Models;
using DKP.Services.Helpers;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace DKP.Services
{
    public static class CommonService
    {
        /// <summary>
        /// 根据字典组code和字典code获取字典名称
        /// </summary>
        /// <param name="groupCode"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string GetDicName(string groupCode, string code)
        {
            return EngineContext.Current.Resolve<DKP.Services.Security.IDictionaryService>().GetNameByCode(groupCode, code);
        }

        public static Dictionary<string, string> GetDic(string groupCode)
        {
            return EngineContext.Current.Resolve<DKP.Services.Security.IDictionaryService>().GetDictionaryByCode(groupCode);
        }

        public static Attachment GetAttachment(int forienKey, int category, string name)
        {
            var attachmentService = EngineContext.Current.Resolve<IAttachmentService>();
            var result = attachmentService.GetAttachment(forienKey, category, name);
            var attachment = new Attachment();
            if (result.Count() > 0)
            {
                attachment = result.First();
            }
            return attachment;
        }

        /// <summary>
        /// 格式化日期
        /// </summary>
        /// <param name="date"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static string FormatDate(DateTime? date, string pattern = "yyyy-MM-dd")
        {
            if (date == null)
            {
                return "";
            }

            return date.Value.ToString(pattern);
        }

        /// <summary>
        /// 转换金额（现金流量测算表使用）
        /// </summary>
        /// <param name="cash"></param>
        /// <param name="lessThen"></param>
        /// <returns></returns>
        public static decimal ToMinZero(this decimal? cash, bool lessThen = true)
        {
            if (cash == null) return 0;

            if (lessThen) { return cash.Value < 0 ? Math.Abs(cash.Value) : 0; }

            return cash.Value > 0 ? cash.Value : 0;
        }

        /// <summary>
        /// 转换金额（现金流量测算表使用）
        /// </summary>
        /// <param name="cash"></param>
        /// <param name="lessThen"></param>
        /// <returns></returns>
        public static string FormatCash(this decimal? cash, string defaultValue)
        {
            if (cash == null) return defaultValue;
            return cash.FormatCash();
        }

        public static string FormatCash(this decimal cash, string defaultValue)
        {
            if (cash == 0) return defaultValue;
            return cash.FormatCash();
        }
        /// <summary>
        /// 格式化金额
        /// </summary>
        /// <param name="cash"></param>
        /// <returns></returns>
        public static string FormatCash(this decimal? cash)
        {
            if (cash == null)
            {
                return "";
            }

            return FormatCash(cash.Value);
        }

        /// <summary>
        /// 格式化金额
        /// </summary>
        /// <param name="cash"></param>
        /// <returns></returns>
        public static string FormatCash(this decimal cash)
        {
            var amountOfDecimalPoint = 2;
            string pattern = "#,##0.";
            for (int i = 0; i < amountOfDecimalPoint; i++)
            {
                pattern += "0";
            }
            return Math.Round(cash, amountOfDecimalPoint, MidpointRounding.AwayFromZero).ToString(pattern);
        }

        public static string FormatCustomCash(this decimal cash,int count)
        {
            string pattern = "#,##0.";
            for (int i = 0; i < count; i++)
            {
                pattern += "0";
            }
            return Math.Round(cash, count, MidpointRounding.AwayFromZero).ToString(pattern);
        }
        /// <summary>
        /// 判断bool?类型的真假
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool checkBool(bool? obj)
        {
            if (obj == null || obj.Value == false)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 根据扩展名获得文件类型的ico
        /// </summary>
        /// <param name="fileExt"></param>
        /// <returns></returns>

        public static string GetIconByFileExt(this string fileExt)
        {
            var icon = "<i class='fa {0}'></i>";
            fileExt = fileExt.Trim().ToLower();
            if (fileExt == ".doc" || fileExt == ".docx") { icon = string.Format(icon, "fa-file-word-o"); }
            else if (fileExt == ".xls" || fileExt == ".xlsx") { icon = string.Format(icon, "fa-file-excel-o"); }
            else if (fileExt == ".pdf") { icon = string.Format(icon, "fa-file-pdf-o"); }
            else if (fileExt == ".txt") { icon = string.Format(icon, "fa-file-text-o"); }
            else { icon = string.Format(icon, "fa-file-o"); }
            return icon;
        }

        public static string CheckPermission(string url,string appPath)
        {
            var u = url;
            if (u == null) return "";
            if (u.IndexOf("?", StringComparison.Ordinal) > 0)
            {
                u = u.Substring(0, u.IndexOf("?", StringComparison.Ordinal));
            }
            var urlList = u.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            var area = "";
            var controller = "";
            var action = "";
            if (urlList.Count() >= 3)
            {
                int last = 0;
                if (!int.TryParse(urlList.Last(), out last) || urlList.Count() == 4)
                {
                    area = urlList[0];
                    controller = urlList[1];
                    action = urlList[2];
                }
                else
                {
                    controller = urlList[0];
                    action = urlList[1];
                }
            }
            else if (urlList.Count() == 2)
            {
                controller = urlList[0];
                action = urlList[1];
            }
            var session = EngineContext.Current.Resolve<IWorkContext>().CurrentUser;
            var permissions = session.Permissions;
            if (permissions == null)
                return "";

            var permission = permissions.Where(p=>!string.IsNullOrWhiteSpace(p.Action) && !string.IsNullOrWhiteSpace(p.Controller)).FirstOrDefault(p => p.Action.ToLower() == action.ToLower() && p.Controller.ToLower() == controller.ToLower() && (string.IsNullOrWhiteSpace(p.Area)?(p.Area == area || p.Area == null) :p.Area.ToLower()==area.ToLower()));

            //当前用户没有此操作权限
            if (permission == null) { return ""; }

            //网站虚拟目录
            
            if (appPath != "/") { appPath = appPath + "/"; }

            url = appPath + url.Substring(1);
            return url;
        }

        public static string GetConString()
        {
            var dataSettingsManager = new DataSettingsManager();
            var dataProviderSettings = dataSettingsManager.LoadSettings();
            return dataProviderSettings.DataConnectionString;
        }
        /// <summary>
        /// 对比季度
        /// </summary>
        /// <param name="date"></param>
        /// <returns>0：无效 1：本季度 2：上季度</returns>
        public static int CheckJD(DateTime date)
        {
            DateTime dt = DateTime.Now.Date;
            DateTime startQuarter = dt.AddMonths(0 - (dt.Month - 1) % 3).AddDays(1 - dt.Day); //本季度初
            DateTime endQuarter = startQuarter.AddMonths(3).AddSeconds(-1); //本季度末

            DateTime pstartQuarter = dt.AddMonths(0 - ((dt.Month - 1) % 3 + 3)).AddDays(1 - dt.Day); //上季度初
            DateTime pendQuarter = pstartQuarter.AddMonths(3).AddSeconds(-1); //上季度末

            if(date>=startQuarter && date <= endQuarter)
            {
                return 1;
            }
            else if (date >= pstartQuarter && date <= pendQuarter)
            {
                return 2;
            }
            else
            {
                return 3;
            }
        }

        public static HttpModel POST(string action,string param = "")
        {
            var key = Guid.NewGuid();
            var appKey = Variable.AppKey;
            var sign = Variable.MD5(key + appKey);
            var result = RequestHelper.HttpPost(Variable.RemoteUrl + action, "key=" + key + "&sign=" + sign + (string.IsNullOrEmpty(param) ? "" : ("&" + param)));
            var httpModel = new HttpModel();
            if (result.StartsWith("接口错误"))
            {
                httpModel.code = 300;
                httpModel.msg = result;
            }
            else
            {
                try
                {
                    httpModel = JsonConvert.DeserializeObject<HttpModel>(result);
                }
                catch (Exception e)
                {
                    httpModel.code = 400;
                    httpModel.msg = "JSON格式化错误：" + (e.InnerException != null ? e.InnerException.Message : e.Message);
                }
            }
            return httpModel;
        }

        public static bool CheckHttpModel(HttpModel httpModel)
        {
            if(httpModel.code == 200)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // 除去所有在html元素中标记
        public static string striphtml(string strhtml)
        {
            string stroutput = strhtml;
            Regex regex = new Regex(@"<[^>]+>|</[^>]+>");
            stroutput = regex.Replace(stroutput, "");
            return stroutput;
        }

    }
}
